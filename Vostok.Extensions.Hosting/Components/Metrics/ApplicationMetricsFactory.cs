using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Hosting.Abstractions;
using Vostok.Logging.Abstractions;
using Vostok.Metrics;
using Vostok.Metrics.Senders;

namespace Vostok.Extensions.Hosting.Components.Metrics;

internal static class ApplicationMetricsFactory
{
    public static IVostokApplicationMetrics Create(IServiceProvider provider)
    {
        var applicationIdentity = provider.GetRequiredService<IVostokApplicationIdentity>();
        var log = provider.GetRequiredService<ILog>();
        var options = provider.GetRequiredService<IOptions<MetricsConfigurationOptions>>().Value;
        var senders = provider.GetServices<IMetricEventSender>().ToArray();

        var metricSender = BuildCompositeMetricEventSender(senders);
        if (metricSender is null)
        {
            log.LogDisabled(nameof(IVostokApplicationMetrics), "not configured metric event senders");
            return new VostokApplicationMetrics(new DevNullMetricContext(), applicationIdentity);
        }

        var annotationSender = BuildCompositeAnnotationEventSender(options, senders);

        var settings = new MetricContextConfig(metricSender)
        {
            ErrorCallback = e => log.ForContext<MetricContext>().Error(e, "Failed to send metrics."),

            AnnotationSender = annotationSender
        };

        options.ApplyCustomizations(settings);

        var root = new MetricContext(settings);

        return new VostokApplicationMetrics(root, applicationIdentity);
    }

    private static IMetricEventSender? BuildCompositeMetricEventSender(IMetricEventSender[] senders) =>
        senders switch
        {
            [] => null,
            [var sender] => sender,
            _ => new CompositeMetricEventSender(senders)
        };

    private static IAnnotationEventSender? BuildCompositeAnnotationEventSender(
        MetricsConfigurationOptions options,
        IEnumerable<IMetricEventSender> metricSenders)
    {
        var senders = metricSenders.OfType<IAnnotationEventSender>().ToArray();

        var annotationSender = senders switch
        {
            [] => null,
            [var sender] => sender,
            _ => new CompositeAnnotationEventSender(senders)
        };

        if (annotationSender != null)
            annotationSender = options.ApplyAnnotationSenderCustomizations(annotationSender);

        return annotationSender;
    }
}