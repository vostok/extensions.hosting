using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http.Headers;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Vostok.Context;
using Vostok.Context.Helpers;
using Vostok.Tracing.Abstractions;

#pragma warning disable CS8618 // disable nullable warnings

namespace Vostok.Extensions.AspNetCore.Tests.OpenTelemetry;

public class VostokContextPropagatorIntegrationTests
{
    private FakeApi fakeApi;
    private ConcurrentDictionary<Guid, ActivityInfo?> activitiesInfo = [];

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        fakeApi = await FakeApi.StartAsync(configureApp: app => app.MapGet("/{requestId:guid}", (Guid requestId) =>
        {
            activitiesInfo[requestId] = Activity.Current is null
                ? null
                : new ActivityInfo
                {
                    TraceId = Activity.Current.TraceId.ToHexString(),
                    ParentSpanId = Activity.Current.ParentSpanId.ToHexString()
                };

            return Results.Ok();
        }));
    }

    [SetUp]
    public void SetUp()
    {
        activitiesInfo = [];
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        if (fakeApi == null!)
            return;
        try
        {
            await fakeApi.StopAsync();
        }
        finally
        {
            await fakeApi.DisposeAsync();
        }
    }

    [Test]
    public async Task SendRequest_WithLegacyTracing_ShouldInferTraceContext()
    {
        var traceContext = CreateTraceContext();
        await TestReceivedTraceContext(headers => AddLegacyTracingHeaders(headers, traceContext),
            expectedActivity: new ActivityInfo(
                TraceFormatter.FormatTraceId(traceContext.TraceId),
                TraceFormatter.FormatLegacySpanId(traceContext.SpanId)
            )
        );
    }

    [Test]
    public async Task SendRequest_WithContextGlobals_ShouldInferTraceContext()
    {
        var traceContext = CreateTraceContext();
        await TestReceivedTraceContext(headers => AddContextGlobalsHeader(headers, traceContext),
            expectedActivity: new ActivityInfo(
                TraceFormatter.FormatTraceId(traceContext.TraceId),
                TraceFormatter.FormatSpanId(traceContext.SpanId)
            )
        );
    }

    [Test]
    public async Task SendRequest_WithTraceparent_ShouldInferTraceContext()
    {
        var traceContext = CreateTraceContext();
        await TestReceivedTraceContext(
            headers => AddTraceparentHeader(headers, traceContext),
            expectedActivity: new ActivityInfo(
                TraceFormatter.FormatTraceId(traceContext.TraceId),
                TraceFormatter.FormatSpanId(traceContext.SpanId)
            )
        );
    }

    [Test]
    public async Task SendRequest_AllFormatsPresent_ShouldInferFromTraceparent()
    {
        var traceContext = CreateTraceContext();
        await TestReceivedTraceContext(headers =>
            {
                AddContextGlobalsHeader(headers, traceContext);
                AddTraceparentHeader(headers, CreateTraceContext());
                AddLegacyTracingHeaders(headers, CreateTraceContext());
            },
            expectedActivity: new ActivityInfo(
                TraceFormatter.FormatTraceId(traceContext.TraceId),
                TraceFormatter.FormatSpanId(traceContext.SpanId)
            )
        );
    }

    [Test]
    public async Task SendRequest_ContextGlobalsAndLegacyContextPresent_ShouldInferFromContextGlobals()
    {
        var traceContext = CreateTraceContext();
        await TestReceivedTraceContext(headers =>
            {
                AddContextGlobalsHeader(headers, traceContext);
                AddLegacyTracingHeaders(headers, CreateTraceContext());
            },
            expectedActivity: new ActivityInfo(
                TraceFormatter.FormatTraceId(traceContext.TraceId),
                TraceFormatter.FormatSpanId(traceContext.SpanId)
            )
        );
    }

    private async Task TestReceivedTraceContext(
        Action<HttpRequestHeaders>? setHeaders,
        ActivityInfo expectedActivity
    )
    {
        var requestId = Guid.NewGuid();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/{requestId}");
        setHeaders?.Invoke(request.Headers);
        if (!request.Headers.Contains("traceparent"))
            request.Headers.Add("traceparent", string.Empty); // traceparent header is populated from Activity so need to suppress it explicitly

        var result = await fakeApi.Client.SendAsync(request);
        result.Should().BeSuccessful();

        activitiesInfo.Should().ContainKey(requestId)
            .WhoseValue.Should().BeEquivalentTo(expectedActivity);
    }

    private void AddLegacyTracingHeaders(HttpRequestHeaders headers, TraceContext traceContext)
    {
        headers.Add("traceparent", "");
        headers.Add("X-Kontur-Trace-Id", TraceFormatter.FormatTraceId(traceContext.TraceId));
        headers.Add("X-Kontur-Trace-SpanId", TraceFormatter.FormatLegacySpanId(traceContext.SpanId, withLeadingZeros: false));
    }

    private void AddContextGlobalsHeader(HttpRequestHeaders headers, TraceContext traceContext)
    {
        string serialized;
        using (FlowingContext.Globals.Use(traceContext))
            serialized = FlowingContext.SerializeDistributedGlobals()!;

        headers.Add("Context-Globals", serialized);
    }

    private void AddTraceparentHeader(HttpRequestHeaders headers, TraceContext traceContext) =>
        headers.Add("traceparent",
            string.Format("00-{0}-{1}-01", TraceFormatter.FormatTraceId(traceContext.TraceId), TraceFormatter.FormatSpanId(traceContext.SpanId))
        );

    private static TraceContext CreateTraceContext() => 
        new(Guid.NewGuid(), Guid.NewGuid());

    private readonly record struct ActivityInfo(
        string TraceId,
        string ParentSpanId
    );
}