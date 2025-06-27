using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Hosting.Abstractions;
using Vostok.Metrics;

namespace Vostok.Extensions.Hosting.Components.Metrics;

[PublicAPI]
public static class VostokBuilderMetricsExtensions
{
    public static IVostokBuilder WithMetrics(
        this IVostokBuilder vostok,
        Action<IVostokMetricsBuilder>? configure = null)
    {
        if (!vostok.CheckComponentAlreadyAdded("Metrics"))
            vostok.Services.AddMetrics();

        var metricsBuilder = new VostokMetricsBuilder(vostok.Services);
        configure?.Invoke(metricsBuilder);

        return vostok;
    }

    private static void AddMetrics(this IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IOptionsFactory<MetricContextConfig>, MetricContextDefaultConfigFactory>());
        services.TryAddSingleton(RootMetricsContextFactory.Create);
        services.TryAddSingleton<IVostokApplicationMetrics, VostokApplicationMetrics>();
    }
}