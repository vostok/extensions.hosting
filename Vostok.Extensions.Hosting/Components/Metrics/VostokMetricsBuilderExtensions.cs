using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Hosting.Abstractions;
using Vostok.Metrics;
using Vostok.Metrics.Models;

namespace Vostok.Extensions.Hosting.Components.Metrics;

[PublicAPI]
public static class VostokMetricsBuilderExtensions
{
    public static IVostokMetricsBuilder Configure(
        this IVostokMetricsBuilder builder,
        Action<MetricContextConfig> configure) =>
        builder.Configure((options, _) => configure(options));

    public static IVostokMetricsBuilder AddLoggingMetricSender(this IVostokMetricsBuilder builder)
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IMetricEventSender, LoggingMetricEventSender>());
        return builder;
    }

    public static IVostokMetricsBuilder CustomizeAnnotationEventSender(
        this IVostokMetricsBuilder builder,
        Func<IAnnotationEventSender, IAnnotationEventSender> customize) =>
        builder.CustomizeAnnotationEventSender((sender, _) => customize(sender));

    /// <inheritdoc cref="EnrichInstanceAnnotationTags(IVostokMetricsBuilder,Vostok.Metrics.Models.MetricTags)" />
    public static IVostokMetricsBuilder EnrichInstanceAnnotationTags(
        this IVostokMetricsBuilder builder,
        params (string key, string value)[] tags)
    {
        var metricTags = tags.Select(t => new MetricTag(t.key, t.value)).ToArray();
        return builder.EnrichInstanceAnnotationTags(new MetricTags(metricTags));
    }

    /// <summary>
    /// Applies given <paramref name="tags"/> to all annotations written by <see cref="IVostokApplicationMetrics.Instance"/> metrics context. 
    /// </summary>
    public static IVostokMetricsBuilder EnrichInstanceAnnotationTags(
        this IVostokMetricsBuilder builder,
        MetricTags tags) =>
        builder.CustomizeAnnotationEventSender(sender => new TagsEnrichingInstanceAnnotationEventSender(sender, tags));
}