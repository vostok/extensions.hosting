using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Vostok.Hosting.Abstractions;
using Vostok.Logging.Abstractions;
using Vostok.Metrics;
using Vostok.Metrics.System.Dns;
using Vostok.Metrics.System.Gc;
using Vostok.Metrics.System.Host;
using Vostok.Metrics.System.Process;

namespace Vostok.Extensions.Hosting.Components.SystemMetrics;

internal sealed class SystemMetricsHostedService(
    GarbageCollectionMonitor gcMonitor,
    DnsMonitor dnsMonitor,
    CurrentProcessMonitor currentProcessMonitor,
    HostMonitor hostMonitor,
    IOptions<SystemMetricsOptions> systemMetricsOptions,
    IVostokApplicationMetrics vostokApplicationMetrics,
    ILog log) : IHostedService, IDisposable
{
    private readonly List<IDisposable> _disposables = [];

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var options = systemMetricsOptions.Value;
        var processMetricsContext = vostokApplicationMetrics.Instance.WithTag("SystemMetricsType", "Process");
        var hostMetricsContext = vostokApplicationMetrics.Instance.WithTag("SystemMetricsType", "Host");

        RegisterGcMonitoring(options, processMetricsContext);
        RegisterDnsMonitoring(options, processMetricsContext);
        RegisterCurrentProcessMonitoring(options, processMetricsContext);
        RegisterHostMonitoring(options, hostMetricsContext);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public void Dispose() =>
        _disposables.ForEach(disposable => disposable.Dispose());

    private void RegisterGcMonitoring(SystemMetricsOptions options, IMetricContext metricContext)
    {
        if (options.EnableGcEventsLogging)
            AddDisposable(gcMonitor.LogCollections(log, gc => gc.Duration >= options.GcMinimumDurationForLogging));
        if (options.EnableGcEventsMetrics)
            AddDisposable(gcMonitor.ReportMetrics(metricContext, options.ProcessMetricsReportingPeriod));
    }

    private void RegisterDnsMonitoring(SystemMetricsOptions options, IMetricContext metricContext)
    {
        if (options.EnableDnsEventsMetrics)
            AddDisposable(dnsMonitor.ReportMetrics(metricContext, options.ProcessMetricsReportingPeriod));
    }

    private void RegisterCurrentProcessMonitoring(SystemMetricsOptions options, IMetricContext metricContext)
    {
        if (options.EnableProcessMetricsLogging)
            AddDisposable(currentProcessMonitor.LogPeriodically(log, options.ProcessMetricsLoggingPeriod));
        if (options.EnableProcessMetricsReporting)
            AddDisposable(currentProcessMonitor.ReportMetrics(metricContext, options.ProcessMetricsReportingPeriod));
    }

    private void RegisterHostMonitoring(SystemMetricsOptions options, IMetricContext metricContext)
    {
        if (options.EnableHostMetricsLogging)
            AddDisposable(hostMonitor.LogPeriodically(log, options.HostMetricsLoggingPeriod));
        if (options.EnableHostMetricsReporting)
            AddDisposable(hostMonitor.ReportMetrics(metricContext, options.HostMetricsReportingPeriod));
    }

    private void AddDisposable(IDisposable disposable) =>
        _disposables.Add(disposable);
}