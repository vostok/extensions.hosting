using Microsoft.Extensions.Logging;

using Vostok.Metrics;
using Vostok.Metrics.Models;

namespace Vostok.Extensions.Hosting.Components.Metrics;

internal sealed class LoggingMetricEventSender(ILoggerFactory loggerFactory)
    : IMetricEventSender, IAnnotationEventSender
{
    private readonly ILogger _logger = loggerFactory.CreateLogger("Vostok.Metrics");

    public void Send(MetricEvent @event)
        => _logger.LogDebug("Metric event: {MetricEvent}", @event);

    public void Send(AnnotationEvent @event)
        => _logger.LogDebug("Annotation event: {AnnotationEvent}", @event);
}