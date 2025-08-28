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
    /// <summary>
    /// Registers and configures services for Vostok metrics components.
    /// </summary>
    /// <param name="vostok">The <see cref="IVostokBuilder"/>.</param>
    /// <param name="configure">Delegate to configure <see cref="IVostokMetricsBuilder"/>.</param>
    /// <returns><see cref="IVostokBuilder"/> for chain method calls.</returns>
    /// <remarks>Calling this method multiple times on the same <see cref="IVostokBuilder"/> instance will result in one metrics components configured.</remarks>
    public static IVostokBuilder WithMetrics(
        this IVostokBuilder vostok,
        Action<IVostokMetricsBuilder>? configure = null)
    {
        if (!vostok.CheckAndMarkComponentAdded("Metrics"))
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