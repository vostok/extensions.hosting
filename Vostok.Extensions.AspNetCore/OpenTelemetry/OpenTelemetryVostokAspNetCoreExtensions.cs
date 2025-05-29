using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Trace;

using Vostok.Extensions.AspNetCore.Middlewares.FillRequestInfo;
using Vostok.ServiceDiscovery.Abstractions;

namespace Vostok.Extensions.AspNetCore.OpenTelemetry;

[PublicAPI]
public static class OpenTelemetryVostokAspNetCoreExtensions
{
    public static void SetupAspNetCoreInstrumentationForVostok(this TracerProviderBuilder tracing) =>
        tracing.ConfigureServices(services => services.AddOptions<AspNetCoreTraceInstrumentationOptions>()
                                                      .PostConfigure<IServiceProvider>(Configure));

    private static void Configure(AspNetCoreTraceInstrumentationOptions options, IServiceProvider provider)
    {
        VostokContextPropagator.Use(provider.GetRequiredService<ILoggerFactory>());

        var fillRequestInfoOptions = provider.GetRequiredService<IOptions<FillRequestInfoMiddlewareOptions>>().Value;
        var serverAddress = GetAddress(provider.GetService<IServiceBeacon>());

        var previousRequestEnrichment = options.EnrichWithHttpRequest;
        options.EnrichWithHttpRequest = (activity, request) =>
        {
            if (serverAddress is var (host, port))
            {
                activity.SetTag(WebSemanticConventions.AttributeServerAddress, host);
                activity.SetTag(WebSemanticConventions.AttributeServerPort, port);
            }

            activity.SetTag(WebSemanticConventions.AttributeClientAddress, request.HttpContext.Connection.RemoteIpAddress);

            var clientName = FillRequestInfoMiddleware.GetClientApplicationIdentity(request, fillRequestInfoOptions);
            if (!string.IsNullOrEmpty(clientName))
                activity.SetTag(WebSemanticConventions.HttpClientName, clientName);

            if (request.ContentLength.HasValue)
                activity.SetTag(WebSemanticConventions.AttributeHttpRequestContentLength, request.ContentLength.Value);

            previousRequestEnrichment?.Invoke(activity, request);
        };

        var previousResponseEnrichment = options.EnrichWithHttpResponse;
        options.EnrichWithHttpResponse = (activity, response) =>
        {
            if (response.ContentLength.HasValue)
                activity.SetTag(WebSemanticConventions.AttributeHttpResponseContentLength, response.ContentLength.Value);

            previousResponseEnrichment?.Invoke(activity, response);
        };
    }

    private static (string host, int port)? GetAddress(IServiceBeacon? beacon)
    {
        if (beacon is null || !beacon.ReplicaInfo.TryGetUrl(out var url))
            return null;

        return (url.Host, url.Port);
    }
}