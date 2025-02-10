using Vostok.Logging.Abstractions;
using Vostok.Metrics;
using Vostok.Metrics.Models;

namespace Vostok.Extensions.Hosting.Components.Metrics;

internal class LoggingMetricEventSender(ILog log) : IMetricEventSender, IAnnotationEventSender
{
    private readonly ILog _log = log.ForContext("Metrics");

    public void Send(MetricEvent @event)
        => _log.Debug("Metric event: {MetricEvent}", @event);

    public void Send(AnnotationEvent @event)
        => _log.Debug("Annotation event: {AnnotationEvent}", @event);
}