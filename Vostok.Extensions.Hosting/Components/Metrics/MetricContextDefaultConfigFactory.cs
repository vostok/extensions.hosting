using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Logging.Abstractions;
using Vostok.Metrics;
using Vostok.Metrics.Senders;

namespace Vostok.Extensions.Hosting.Components.Metrics;

internal sealed class MetricContextDefaultConfigFactory(
    IEnumerable<IConfigureOptions<MetricContextConfig>> setups,
    IEnumerable<IPostConfigureOptions<MetricContextConfig>> postConfigures,
    IEnumerable<IValidateOptions<MetricContextConfig>> validations,
    IServiceProvider provider) : OptionsFactory<MetricContextConfig>(setups, postConfigures, validations)
{
    protected override MetricContextConfig CreateInstance(string _)
    {
        var log = provider.GetService<ILog>();
        var senders = provider.GetServices<IMetricEventSender>().ToArray();

        var compositeMetricEventSender = BuildCompositeMetricEventSender(senders);
        var annotationEventSender = BuildCompositeAnnotationEventSender(senders);

        return new MetricContextConfig(compositeMetricEventSender)
        {
            ErrorCallback = e => log?.ForContext<MetricContext>().Error(e, "Failed to send metrics."),
            AnnotationSender = annotationEventSender
        };
    }

    private static IMetricEventSender BuildCompositeMetricEventSender(IMetricEventSender[] senders) =>
        senders switch
        {
            [] => new DevNullMetricEventSender(),
            [var sender] => sender,
            _ => new CompositeMetricEventSender(senders)
        };

    private static IAnnotationEventSender? BuildCompositeAnnotationEventSender(IMetricEventSender[] senders)
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