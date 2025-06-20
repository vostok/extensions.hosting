using JetBrains.Annotations;

namespace Vostok.Extensions.Hosting.Components.SystemMetrics;

[PublicAPI]
public static class SystemMetricsOptionsExtensions
{
    public static void DisableAll(this SystemMetricsOptions options)
    {
        options.DisableLogging();

        options.EnableGcEventsMetrics = false;
        options.EnableDnsEventsMetrics = false;
        options.EnableProcessMetricsReporting = false;
        options.EnableHostMetricsReporting = false;
    }

    public static void EnableAll(this SystemMetricsOptions options)
    {
        options.EnableLogging();

        options.EnableGcEventsMetrics = true;
        options.EnableDnsEventsMetrics = true;
        options.EnableProcessMetricsReporting = true;
        options.EnableHostMetricsReporting = true;
    }

    public static void DisableLogging(this SystemMetricsOptions options)
    {
        options.EnableGcEventsLogging = false;
        options.EnableProcessMetricsLogging = false;
        options.EnableHostMetricsLogging = false;
    }

    public static void EnableLogging(this SystemMetricsOptions options)
    {
        options.EnableGcEventsLogging = true;
        options.EnableProcessMetricsLogging = true;
        options.EnableHostMetricsLogging = true;
    }
}