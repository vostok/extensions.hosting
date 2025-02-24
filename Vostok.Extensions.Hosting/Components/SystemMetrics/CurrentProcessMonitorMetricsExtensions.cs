using Vostok.Metrics;
using Vostok.Metrics.System.Host;
using Vostok.Metrics.System.Process;

namespace Vostok.Extensions.Hosting.Components.SystemMetrics;

internal static class CurrentProcessMonitorMetricsExtensions
{
    public static IDisposable ReportMetrics(
        this CurrentProcessMonitor currentProcessMonitor,
        IMetricContext metricContext,
        TimeSpan period) =>
        currentProcessMonitor.ObserveMetrics(period).Subscribe(new CurrentProcessMetricsReporter(metricContext));

    public static IDisposable ReportMetrics(
        this HostMonitor currentProcessMonitor,
        IMetricContext metricContext,
        TimeSpan period) =>
        currentProcessMonitor.ObserveMetrics(period).Subscribe(new HostMetricsReporter(metricContext));

    private sealed class CurrentProcessMetricsReporter(IMetricContext metricContext) : IObserver<CurrentProcessMetrics>
    {
        public void OnNext(CurrentProcessMetrics value)
        {
            foreach (var metricDataPoint in value.ToDataPoints())
                metricContext.Send(metricDataPoint);
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }
    }

    private sealed class HostMetricsReporter(IMetricContext metricContext) : IObserver<HostMetrics>
    {
        public void OnNext(HostMetrics value)
        {
            foreach (var metricDataPoint in value.ToDataPoints())
                metricContext.Send(metricDataPoint);
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }
    }
}