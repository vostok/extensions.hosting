using JetBrains.Annotations;

using Vostok.Metrics.System.Host;
using Vostok.Metrics.System.Process;

namespace Vostok.Extensions.Hosting.Components.SystemMetrics;

[PublicAPI]
public class SystemMetricsOptions
{
    public bool EnableGcEventsLogging { get; set; } = true;

    public bool EnableGcEventsMetrics { get; set; } = true;

    public bool EnableDnsEventsMetrics { get; set; } = true;

    public bool EnableProcessMetricsLogging { get; set; } = true;

    public bool EnableProcessMetricsReporting { get; set; } = true;

    public bool EnableHostMetricsLogging { get; set; }

    public bool EnableHostMetricsReporting { get; set; }

    public HostMetricsSettings HostMetricsSettings { get; set; } = new();

    public LinuxProcessMetricsSettings LinuxProcessMetricsSettings { get; set; } = new();

    public TimeSpan GcMinimumDurationForLogging { get; set; } = TimeSpan.FromMilliseconds(500);

    public TimeSpan ProcessMetricsLoggingPeriod { get; set; } = TimeSpan.FromSeconds(10);

    public TimeSpan ProcessMetricsReportingPeriod { get; set; } = TimeSpan.FromSeconds(10);

    public TimeSpan HostMetricsLoggingPeriod { get; set; } = TimeSpan.FromSeconds(10);

    public TimeSpan HostMetricsReportingPeriod { get; set; } = TimeSpan.FromSeconds(10);
}