using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Hosting.Abstractions;
using Vostok.Metrics.System.Host;
using Vostok.Metrics.System.Process;

namespace Vostok.Extensions.Hosting.Components.SystemMetrics;

internal static class SystemMetricsFactory
{
    public static CurrentProcessMonitor CreateProcessMonitor(IServiceProvider provider)
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
    }

    public static HostMonitor CreateHostMonitor(IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<SystemMetricsOptions>>().Value;
        return new HostMonitor(options.HostMetricsSettings);
    }
}