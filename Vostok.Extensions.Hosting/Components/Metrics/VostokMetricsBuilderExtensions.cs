using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Metrics;

namespace Vostok.Extensions.Hosting.Components.Metrics;

[PublicAPI]
public static class VostokMetricsBuilderExtensions
{
    public static IVostokMetricsBuilder Configure(
        this IVostokMetricsBuilder builder,
        Action<MetricContextConfig> configure)
    {
        builder.Configure((options, _) => configure(options));
        return builder;
    }

    public static IVostokMetricsBuilder Configure(
        this IVostokMetricsBuilder builder,
        Action<MetricContextConfig, IServiceProvider> configure)
    {
        builder.Services.AddOptions<MetricContextConfig>().Configure(configure);
        return builder;
    }

    public static IVostokMetricsBuilder AddMetricSender(
        this IVostokMetricsBuilder builder,
        Func<IServiceProvider, IMetricEventSender> implementationFactory)
    {
        builder.Services.AddSingleton(implementationFactory);
        return builder;
    }

    public static IVostokMetricsBuilder AddLoggingMetricSender(this IVostokMetricsBuilder builder)
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IMetricEventSender, LoggingMetricEventSender>());
        return builder;
    }

    public static IVostokMetricsBuilder ClearMetricSenders(this IVostokMetricsBuilder builder)
    {
        builder.Services.RemoveAll<IMetricEventSender>();
        return builder;
    }
}