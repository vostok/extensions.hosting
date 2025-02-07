using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.Components.Metrics.Hercules;
using Vostok.Metrics;
using Vostok.Metrics.Hercules;

namespace Vostok.Extensions.Hosting.Components.Metrics;

[PublicAPI]
public static class VostokMetricsBuilderExtensions
{
    public static IVostokMetricsBuilder Configure(
        this IVostokMetricsBuilder builder,
        Action<MetricsConfigurationOptions> configure)
    {
        builder.Configure((options, _) => configure(options));
        return builder;
    }

    public static IVostokMetricsBuilder Configure(
        this IVostokMetricsBuilder builder,
        Action<MetricsConfigurationOptions, IServiceProvider> configure)
    {
        builder.Services.AddOptions<MetricsConfigurationOptions>().Configure(configure);
        return builder;
    }

    public static IVostokMetricsBuilder AddMetricSender(
        this IVostokMetricsBuilder builder,
        IMetricEventSender metricEventSender)
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton(metricEventSender));
        return builder;
    }

    public static IVostokMetricsBuilder AddLoggingMetricSender(this IVostokMetricsBuilder builder)
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IMetricEventSender, LoggingMetricEventSender>());
        return builder;
    }

    public static IVostokMetricsBuilder AddHerculesMetricSender(
        this IVostokMetricsBuilder builder,
        Action<HerculesMetricsSenderConfigurationOptions> configure)
    {
        builder.AddHerculesMetricSender((options, _) => configure(options));
        return builder;
    }
    
    public static IVostokMetricsBuilder AddHerculesMetricSender(
        this IVostokMetricsBuilder builder,
        Action<HerculesMetricsSenderConfigurationOptions, IServiceProvider>? configure = null)
    {
        var serviceDescriptor =
            ServiceDescriptor.Singleton<IMetricEventSender, HerculesMetricSender>(HerculesMetricSenderFactory.Create);

        builder.Services.TryAddEnumerable(serviceDescriptor);

        if (configure is not null)
            builder.Services.AddOptions<HerculesMetricsSenderConfigurationOptions>().Configure(configure);

        return builder;
    }

    public static IVostokMetricsBuilder ClearMetricSenders(this IVostokMetricsBuilder builder)
    {
        builder.Services.RemoveAll<IVostokMetricsBuilder>();
        return builder;
    }
}