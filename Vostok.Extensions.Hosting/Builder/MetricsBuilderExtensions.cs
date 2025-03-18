using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Components.Metrics;
using Vostok.Extensions.Hosting.Components.Metrics.Options;
using Vostok.Hosting.Abstractions;
using Vostok.Metrics;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class MetricsBuilderExtensions
{
    public static IVostokBuilder WithMetrics(
        this IVostokBuilder builder,
        Action<IVostokMetricsBuilder>? configure = null)
    {
        builder.Services.AddMetrics();

        var metricsBuilder = new VostokMetricsBuilder(builder.Services);
        configure?.Invoke(metricsBuilder);

        return builder;
    }

    private static void AddMetrics(this IServiceCollection services)
    {
        services.TryAddSingleton<IOptionsFactory<MetricContextConfig>, MetricContextConfigFactory>();
        services.TryAddSingleton<IConfigureOptions<MetricContextConfig>, MetricContextDefaultConfiguration>();
        services.TryAddSingleton(RootMetricsContextFactory.Create);
        services.TryAddSingleton<IVostokApplicationMetrics, VostokApplicationMetrics>();
    }
}