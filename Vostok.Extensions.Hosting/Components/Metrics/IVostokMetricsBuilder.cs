using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Vostok.Metrics;

namespace Vostok.Extensions.Hosting.Components.Metrics;

/// <summary>
/// Builder for configuring Vostok metrics components.
/// </summary>
[PublicAPI]
public interface IVostokMetricsBuilder
{
    /// <summary>
    /// The host <see cref="IServiceCollection"/> where Vostok metrics components are configured.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Configures <see cref="MetricContextConfig"/>.
    /// </summary>
    /// <param name="configure">Delegate to configure <see cref="MetricContextConfig"/>.</param>
    /// <returns><see cref="IVostokMetricsBuilder"/> for chain method calls.</returns>
    IVostokMetricsBuilder Configure(Action<MetricContextConfig> configure);

    /// <summary>
    /// Add provided <see cref="IMetricEventSender"/>.
    /// </summary>
    /// <param name="implementationFactory">The <see cref="IMetricEventSender"/> implementation factory.</param>
    /// <returns><see cref="IVostokMetricsBuilder"/> for chain method calls.</returns>
    IVostokMetricsBuilder AddMetricSender(Func<IServiceProvider, IMetricEventSender> implementationFactory);

    /// <summary>
    /// Clears all registered <see cref="IMetricEventSender"/>s.
    /// </summary>
    /// <returns><see cref="IVostokMetricsBuilder"/> for chain method calls.</returns>
    IVostokMetricsBuilder ClearMetricSenders();

    /// <summary>
    /// Customizes <see cref="IAnnotationEventSender"/>.
    /// </summary>
    /// <param name="senderCustomization">Delegate to customize <see cref="IAnnotationEventSender"/>.</param>
    /// <returns><see cref="IVostokMetricsBuilder"/> for chain method calls.</returns>
    IVostokMetricsBuilder CustomizeAnnotationEventSender(Func<IAnnotationEventSender, IServiceProvider, IAnnotationEventSender> senderCustomization);
}