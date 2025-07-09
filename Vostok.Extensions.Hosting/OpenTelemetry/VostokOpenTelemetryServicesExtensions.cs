using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

using Vostok.Tracing.Diagnostics;

namespace Vostok.Extensions.Hosting.OpenTelemetry;

internal static class VostokOpenTelemetryServicesExtensions
{
    public static void ConfigureOpenTelemetryForVostok(this IServiceCollection services) =>
        services.ConfigureOpenTelemetryTracerProvider(Configure)
                .ConfigureOpenTelemetryMeterProvider(Configure);

    private static void Configure(TracerProviderBuilder tracing) =>
        tracing.AddSource(TracingConstants.VostokTracerActivitySourceName);

    private static void Configure(MeterProviderBuilder metrics) =>
        metrics.ConfigureServices(services => services.TryAddSingleton<VostokMeters>())
               .AddMeter("Vostok.Metrics.*");
}