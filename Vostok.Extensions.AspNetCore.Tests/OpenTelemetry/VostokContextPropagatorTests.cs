using System.Diagnostics;

using Microsoft.Extensions.Logging;

using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

using Vostok.Context;
using Vostok.Extensions.AspNetCore.OpenTelemetry;
using Vostok.Tracing;
using Vostok.Tracing.Abstractions;

namespace Vostok.Extensions.AspNetCore.Tests.OpenTelemetry;

internal sealed class VostokContextPropagatorTests
{
    private VostokContextPropagator _vostokContextPropagator = null!;
    private TraceContext _traceContext = null!;

    private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(_ => { });

    [SetUp]
    public void SetUp()
    {
        FlowingContext.Configuration.ClearDistributedGlobals();
        FlowingContext.Configuration.RegisterDistributedGlobal(VostokContextReader.DistributedGlobalName,
            new TraceContextSerializer());

        _vostokContextPropagator = new VostokContextPropagator(_loggerFactory);
        _traceContext = new TraceContext(Guid.NewGuid(), Guid.NewGuid());
    }

    [Test]
    public void Should_extract_trace_data_from_context_globals()
    {
        FlowingContext.Globals.Use(_traceContext);

        var serialized = FlowingContext.SerializeDistributedGlobals()!;

        var result = Extract(new() { { VostokContextPropagator.ContextGlobalsHeader, serialized } });
        CheckResult(result);
    }

    [Test]
    public void Should_extract_trace_data_from_context_globals_and_skip_other_fields()
    {
        FlowingContext.Configuration.RegisterDistributedGlobal("int", new IntSerializer());
        FlowingContext.Configuration.RegisterDistributedGlobal("string", new StringSerializer());

        FlowingContext.Globals.Use(42);
        FlowingContext.Globals.Use("Hello World!");
        FlowingContext.Globals.Use(_traceContext);

        var serialized = FlowingContext.SerializeDistributedGlobals()!;

        var result = Extract(new() { { VostokContextPropagator.ContextGlobalsHeader, serialized } });
        CheckResult(result);
    }

    [Test]
    public void Should_extract_trace_data_from_context_globals_and_skip_fields_with_same_size()
    {
        FlowingContext.Configuration.RegisterDistributedGlobal("vostok.tracing.contaaa", new StringSerializer());

        FlowingContext.Globals.Use("Hello World!");
        FlowingContext.Globals.Use(_traceContext);

        var serialized = FlowingContext.SerializeDistributedGlobals()!;

        var result = Extract(new() { { VostokContextPropagator.ContextGlobalsHeader, serialized } });
        CheckResult(result);
    }

    [Test]
    public void Should_not_extract_trace_data_from_context_globals_when_not_present()
    {
        var serialized = FlowingContext.SerializeDistributedGlobals()!;

        var result = Extract(new() { { VostokContextPropagator.ContextGlobalsHeader, serialized } });
        result.ActivityContext.IsValid().Should().BeFalse();
    }

    [Test]
    public void Should_extract_trace_data_from_legacy_headers()
    {
        var result = Extract(new()
        {
            { VostokContextPropagator.XKonturTraceId, TraceFormatter.FormatTraceId(_traceContext.TraceId) },
            { VostokContextPropagator.XKonturTraceSpanId, TraceFormatter.FormatLegacySpanId(_traceContext.SpanId, withLeadingZeros: false) }
        });
        CheckResult(result, useLegacySpanId: true);
    }

    [Test]
    public void Should_extract_trace_date_with_context_globals_precedence()
    {
        FlowingContext.Globals.Use(_traceContext);

        var serialized = FlowingContext.SerializeDistributedGlobals()!;

        var result = Extract(new()
        {
            { VostokContextPropagator.ContextGlobalsHeader, serialized },
            { VostokContextPropagator.XKonturTraceId, TraceFormatter.FormatTraceId(Guid.NewGuid()) },
            { VostokContextPropagator.XKonturTraceSpanId, TraceFormatter.FormatLegacySpanId(Guid.NewGuid(), withLeadingZeros: false) }
        });
        CheckResult(result);
    }

    private void CheckResult(PropagationContext result, bool useLegacySpanId = false)
    {
        result.ActivityContext.Should().BeEquivalentTo(new ActivityContext(
            traceId: ActivityTraceId.CreateFromString(TraceFormatter.FormatTraceId(_traceContext.TraceId)),
            spanId: ActivitySpanId.CreateFromString(useLegacySpanId 
                ? TraceFormatter.FormatLegacySpanId(_traceContext.SpanId) 
                : TraceFormatter.FormatSpanId(_traceContext.SpanId)
            ),
            traceFlags: ActivityTraceFlags.Recorded,
            traceState: null,
            isRemote: true
        ));
    }

    private PropagationContext Extract(Dictionary<string, string?> values) =>
        _vostokContextPropagator.Extract(default, values, static (carrier, key) =>
            carrier.TryGetValue(key, out var value) && value != null
                ? [value]
                : []
        );

    private sealed class IntSerializer : IContextSerializer<int>
    {
        public string Serialize(int value) =>
            value.ToString();

        public int Deserialize(string input) =>
            int.Parse(input);
    }

    private sealed class StringSerializer : IContextSerializer<string>
    {
        public string Serialize(string value) =>
            value;

        public string Deserialize(string input) =>
            input;
    }
}