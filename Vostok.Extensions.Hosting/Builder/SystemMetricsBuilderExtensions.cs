using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Components.SystemMetrics;
using Vostok.Hosting.Abstractions;
using Vostok.Metrics.System.Dns;
using Vostok.Metrics.System.Gc;
using Vostok.Metrics.System.Host;
using Vostok.Metrics.System.Process;

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
        services.TryAddSingleton<CurrentProcessMonitor>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<SystemMetricsOptions>>().Value;
            var applicationLimits = provider.GetRequiredService<IVostokApplicationLimits>();
            
            var settings = new CurrentProcessMetricsSettings
            {
                CpuCoresLimitProvider = () => applicationLimits.CpuUnits,
                MemoryBytesLimitProvider = () => applicationLimits.MemoryBytes,
                LinuxSettings = options.LinuxProcessMetricsSettings
            };
            
            return new CurrentProcessMonitor(settings);
        });
        services.TryAddSingleton<HostMonitor>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<SystemMetricsOptions>>().Value;
            return new HostMonitor(options.HostMetricsSettings);
        });

        services.AddHostedService<SystemMetricHostedService>();
    }
}