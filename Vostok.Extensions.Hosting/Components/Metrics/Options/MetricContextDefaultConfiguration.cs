using Microsoft.Extensions.Options;

using Vostok.Logging.Abstractions;
using Vostok.Metrics;
using Vostok.Metrics.Senders;

namespace Vostok.Extensions.Hosting.Components.Metrics.Options;

internal class MetricContextDefaultConfiguration(ILog log, IEnumerable<IMetricEventSender> senders)
    : IConfigureOptions<MetricContextConfig>
{
    public void Configure(MetricContextConfig config)
    {
        config.ErrorCallback = e => log.ForContext<MetricContext>().Error(e, "Failed to send metrics.");

        var annotationEventSender = BuildCompositeAnnotationEventSender();
        config.AnnotationSender = annotationEventSender;
    }

    private IAnnotationEventSender? BuildCompositeAnnotationEventSender()
    {
        var annotationSenders = senders.OfType<IAnnotationEventSender>().ToArray();

        var annotationSender = annotationSenders switch
        {
            [] => null,
            [var sender] => sender,
            _ => new CompositeAnnotationEventSender(annotationSenders)
        };

        return annotationSender;
    }
}