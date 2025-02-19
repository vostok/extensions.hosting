using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using Vostok.Extensions.Hosting.OpenTelemetry.ResourceDetectors;

namespace Vostok.Extensions.Hosting.OpenTelemetry;

internal static class VostokOpenTelemetryServicesExtensions
{
    public static void ConfigureOpenTelemetryForVostok(this IVostokBuilder builder) =>
        builder.Services
               .ConfigureOpenTelemetryTracerProvider(Configure)
               .ConfigureOpenTelemetryMeterProvider(Configure)
               .ConfigureOpenTelemetryLoggerProvider(Configure);

    private static void Configure(TracerProviderBuilder tracing) =>
        tracing.AddSource(TelemetryConstants.VostokTracerActivitySourceName)
               .SetSampler(new AlwaysOnSampler())
               .ConfigureResource(ConfigureTracingResource);

    private static void Configure(MeterProviderBuilder metrics) =>
        metrics.AddMeter(TelemetryConstants.VostokLogMeterName)
               .ConfigureResource(ConfigureMetricsResource);

    private static void Configure(LoggerProviderBuilder logging) =>
        logging.ConfigureServices(services => services.Configure<OpenTelemetryLoggerOptions>(options =>
               {
                   options.IncludeFormattedMessage = true;
               }))
               .ConfigureResource(ConfigureLoggingResource);

    private static void ConfigureTracingResource(ResourceBuilder resourceBuilder) =>
        resourceBuilder.Clear()
                       .AddTelemetrySdk()
                       .AddDetector(provider => new ServiceResourceDetector(provider));

    private static void ConfigureMetricsResource(ResourceBuilder resourceBuilder)
    {
        resourceBuilder.Clear()
                       .AddDetector(provider =>
                       {
                           var options = provider.GetRequiredService<IOptions<VostokOpenTelemetryMeterResourceOptions>>();
                           return new VostokIdentityResourceDetector(provider, ToDetectorOptions(options.Value));
                       });
        return;

        static VostokIdentityResourceDetector.AttributesOptions ToDetectorOptions(VostokOpenTelemetryMeterResourceOptions options) =>
            new()
            {
                AddProject = options.AddProject,
                AddSubproject = options.AddSubproject,
                AddEnvironment = options.AddEnvironment,
                AddApplication = options.AddApplication,
                AddInstance = options.AddInstance
            };
    }

    private static void ConfigureLoggingResource(ResourceBuilder resourceBuilder) =>
        resourceBuilder.Clear()
                       .AddTelemetrySdk()
                       .AddDetector(provider => new ServiceResourceDetector(provider))
                       .AddDetector(provider => new VostokIdentityResourceDetector(provider));
}