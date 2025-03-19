using System.Text;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Hercules.Client;
using Vostok.Hercules.Client.Abstractions;
using Vostok.Hosting.Abstractions;
using Vostok.Metrics;
using Vostok.Metrics.Models;
using Vostok.Metrics.Primitives.Gauge;

namespace Vostok.Extensions.Hosting.Components.Hercules;

// todo: Replace with OTel metrics
internal sealed class HerculesSinkMetricsHostedService(IServiceProvider provider, ILogger<HerculesSink> logger)
    : IHostedService
{
    private HerculesSink _herculesSink = null!;
    private MetricTags _tags = null!;

    private volatile HerculesSinkStatistics _previous =
        new(HerculesSinkCounters.Zero, new Dictionary<string, HerculesSinkCounters>());

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var sink = provider.GetService<IHerculesSink>();
        var metrics = provider.GetService<IVostokApplicationMetrics>();

        if (sink is not HerculesSink actualSink || metrics is null)
            return Task.CompletedTask;

        var builtContext = metrics.Application.WithTag(WellKnownTagKeys.Component, "HerculesSink");

        _herculesSink = actualSink;
        _tags = builtContext.Tags;

        var registration = builtContext.CreateMultiFuncGaugeFromEvents(ProvideMetrics, new FuncGaugeConfig { ScrapeOnDispose = true }) as IDisposable;
        provider.RegisterDisposable(registration!);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private IEnumerable<MetricEvent> ProvideMetrics()
    {
        var timestamp = DateTimeOffset.Now;

        var statistic = _herculesSink.GetStatistics();

        var delta = statistic.Total - _previous.Total;

        LogSentRecords(statistic, delta);

        _previous = statistic;

        yield return CreateMetricEvent(timestamp, "RecordsLostDueToBuildFailures", delta.RecordsLostDueToBuildFailures);
        yield return CreateMetricEvent(timestamp, "RecordsLostDueToOverflows", delta.RecordsLostDueToOverflows);
        yield return CreateMetricEvent(timestamp, "RecordsLostDueToSizeLimit", delta.RecordsLostDueToSizeLimit);
        yield return CreateMetricEvent(timestamp, "TotalLostRecords", delta.TotalLostRecords);
        yield return CreateMetricEvent(timestamp, "RejectedRecordsCount", delta.RejectedRecords.Count);
        yield return CreateMetricEvent(timestamp, "RejectedRecordsSize", delta.RejectedRecords.Size);
        yield return CreateMetricEvent(timestamp, "SentRecordsSize", delta.SentRecords.Size);
        yield return CreateMetricEvent(timestamp, "SentRecordsCount", delta.SentRecords.Count);
        yield return CreateMetricEvent(timestamp, "StoredRecordsSize", statistic.Total.StoredRecords.Size);
        yield return CreateMetricEvent(timestamp, "StoredRecordsCount", statistic.Total.StoredRecords.Count);
        yield return CreateMetricEvent(timestamp, "Capacity", statistic.Total.Capacity);
    }

    private void LogSentRecords(HerculesSinkStatistics statistic, HerculesSinkCounters delta)
    {
        var details = new StringBuilder();
        foreach (var kvp in statistic.PerStream)
        {
            _previous.PerStream.TryGetValue(kvp.Key, out var p);
            var count = kvp.Value.SentRecords.Count - (p?.SentRecords.Count ?? 0);
            if (count == 0)
                continue;

            details.Append(details.Length > 0 ? ", " : " (");
            details.Append($"{count} {kvp.Key}");
        }

        if (details.Length > 0)
            details.Append(')');

        logger.LogInformation("Successfully sent {RecordsCount} record(s) of size {RecordsSize}{Details}.",
            delta.SentRecords.Count,
            delta.SentRecords.Size,
            details.ToString());
    }

    private MetricEvent CreateMetricEvent(DateTimeOffset timestamp, string name, double value)
    {
        return new MetricEvent(
            value,
            _tags
                .Append("summarized", "Total") //may be by stream
                .Append(WellKnownTagKeys.Name, name),
            timestamp,
            null,
            WellKnownAggregationTypes.Counter,
            null);
    }
}