using Vostok.Commons.Collections;
using Vostok.Hosting.Abstractions.Diagnostics;
using Vostok.Metrics;
using Vostok.Metrics.Models;

namespace Vostok.Extensions.Hosting.Components.Diagnostics.InfoProviders;

internal sealed class ApplicationMetricsProvider : IDiagnosticInfoProvider, IMetricEventSender
{
    private const int BufferCapacity = 1000;

    private static readonly HashSet<string> AllowedAggregationTypes = [WellKnownAggregationTypes.Counter];

    private readonly CircularBuffer<MetricEvent> _events = new(BufferCapacity);
    private readonly object _sync = new();

    public void Send(MetricEvent @event)
    {
        if (!string.IsNullOrEmpty(@event.AggregationType) && !AllowedAggregationTypes.Contains(@event.AggregationType))
            return;

        lock (_sync)
            _events.Add(@event);
    }

    public object Query()
    {
        MetricEvent[] snapshot;

        lock (_sync)
            snapshot = _events.ToArray();

        var result = new Dictionary<string, double>();

        foreach (var @event in snapshot)
            result[GetFlatMetricName(@event)] = @event.Value;

        return result.OrderBy(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    private static string GetFlatMetricName(MetricEvent @event)
        => string.Join(".", @event.Tags.Select(tag => tag.Value));
}