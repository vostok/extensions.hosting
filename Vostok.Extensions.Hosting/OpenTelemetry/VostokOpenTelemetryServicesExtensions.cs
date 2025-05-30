using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using Vostok.Extensions.Hosting.OpenTelemetry.ResourceDetectors;
using Vostok.Tracing.Diagnostics;

namespace Vostok.Extensions.Hosting.OpenTelemetry;

internal static class VostokOpenTelemetryServicesExtensions
{
    public static void ConfigureOpenTelemetryForVostok(this IVostokBuilder builder) =>
        builder.Services
               .ConfigureOpenTelemetryTracerProvider(Configure)
               .ConfigureOpenTelemetryMeterProvider(Configure)
               .ConfigureOpenTelemetryLoggerProvider(Configure);

    private static void Configure(TracerProviderBuilder tracing) =>
        tracing.AddSource(TracingConstants.VostokTracerActivitySourceName)
               .SetSampler(new AlwaysOnSampler())
               .ConfigureResource(ConfigureTracingResource);

    private static void Configure(MeterProviderBuilder metrics) =>
        metrics.ConfigureServices(services => services.TryAddSingleton<VostokMeters>())
               .AddMeter("Vostok.Metrics.*")
               .ConfigureResource(ConfigureMetricsResource);

    private static void Configure(LoggerProviderBuilder logging) =>
        logging.ConfigureServices(services => services.Configure<OpenTelemetryLoggerOptions>(options =>
               {
                   options.IncludeFormattedMessage = true;
                   options.IncludeScopes = true;
               }))
               .ConfigureResource(ConfigureLoggingResource);

    private static void ConfigureTracingResource(ResourceBuilder resourceBuilder) =>
        resourceBuilder.Clear()
                       .AddDetector(provider => new ServiceResourceDetector(provider))
                       .AddDetector(new HostNameResourceDetector(VostokSemanticConventions.AttributeHostName));

    private static void ConfigureMetricsResource(ResourceBuilder resourceBuilder) =>
        resourceBuilder.Clear();

    private static void ConfigureLoggingResource(ResourceBuilder resourceBuilder) =>
        resourceBuilder.Clear()
                       .AddDetector(provider => new VostokIdentityResourceDetector(provider))
                       .AddDetector(new HostNameResourceDetector(VostokSemanticConventions.AttributeHostNameLegacy));
}