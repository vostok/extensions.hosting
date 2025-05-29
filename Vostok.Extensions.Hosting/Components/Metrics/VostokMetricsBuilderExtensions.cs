using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Components.Metrics.Hercules;
using Vostok.Metrics;
using Vostok.Metrics.Hercules;

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
        Action<HerculesMetricSenderSettings> configure)
    {
        builder.AddHerculesMetricSender((options, _) => configure(options));
        return builder;
    }

    public static IVostokMetricsBuilder AddHerculesMetricSender(
        this IVostokMetricsBuilder builder,
        Action<HerculesMetricSenderSettings, IServiceProvider>? configure = null)
    {
        var serviceDescriptor = ServiceDescriptor.Singleton<IMetricEventSender, HerculesMetricSender>(HerculesMetricSenderFactory.Create);
        builder.Services.TryAddEnumerable(serviceDescriptor);
        builder.Services.TryAddSingleton<IOptionsFactory<HerculesMetricSenderSettings>, HerculesMetricSenderDefaultSettingsFactory>();

        if (configure is not null)
            builder.Services.AddOptions<HerculesMetricSenderSettings>().Configure(configure);

        return builder;
    }

    public static IVostokMetricsBuilder ClearMetricSenders(this IVostokMetricsBuilder builder)
    {
        builder.Services.RemoveAll<IMetricEventSender>();
        return builder;
    }
}