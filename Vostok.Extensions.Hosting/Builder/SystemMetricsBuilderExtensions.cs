using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.Components.SystemMetrics;
using Vostok.Metrics.System.Dns;
using Vostok.Metrics.System.Gc;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class SystemMetricsBuilderExtensions
{
    public static IVostokBuilder WithSystemMetrics(
        this IVostokBuilder builder,
        Action<SystemMetricsOptions> configure)
    {
        builder.WithSystemMetrics((options, _) => configure(options));
        return builder;
    }

    public static IVostokBuilder WithSystemMetrics(
        this IVostokBuilder builder,
        Action<SystemMetricsOptions, IServiceProvider>? configure = null)
    {
        builder.Services.AddSystemMetrics();

        if (configure is not null)
            builder.Services.AddOptions<SystemMetricsOptions>().Configure(configure);

        return builder;
    }

    private static void AddSystemMetrics(this IServiceCollection services)
    {
        // todo: Check system metrics enabled

        services.TryAddSingleton<GarbageCollectionMonitor>();
        services.TryAddSingleton<DnsMonitor>();
        services.TryAddSingleton(SystemMetricsFactory.CreateProcessMonitor);
        services.TryAddSingleton(SystemMetricsFactory.CreateHostMonitor);

        services.AddHostedService<SystemMetricsHostedService>();
    }
}