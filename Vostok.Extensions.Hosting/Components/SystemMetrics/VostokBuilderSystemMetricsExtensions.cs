using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Metrics.System.Dns;
using Vostok.Metrics.System.Gc;

namespace Vostok.Extensions.Hosting.Components.SystemMetrics;

[PublicAPI]
public static class VostokBuilderSystemMetricsExtensions
{
    public static IVostokBuilder WithSystemMetrics(
        this IVostokBuilder vostok,
        Action<SystemMetricsOptions> configure)
    {
        vostok.WithSystemMetrics((options, _) => configure(options));
        return vostok;
    }

    public static IVostokBuilder WithSystemMetrics(
        this IVostokBuilder vostok,
        Action<SystemMetricsOptions, IServiceProvider>? configure = null)
    {
        if (!vostok.CheckComponentAlreadyAdded("SystemMetrics"))
            vostok.Services.AddSystemMetrics();

        if (configure is not null)
            vostok.Services.AddOptions<SystemMetricsOptions>().Configure(configure);

        return vostok;
    }

    private static void AddSystemMetrics(this IServiceCollection services)
    {
        services.TryAddSingleton<GarbageCollectionMonitor>();
        services.TryAddSingleton<DnsMonitor>();
        services.TryAddSingleton(SystemMetricsFactory.CreateProcessMonitor);
        services.TryAddSingleton(SystemMetricsFactory.CreateHostMonitor);

        services.AddHostedService<SystemMetricsHostedService>();
    }
}