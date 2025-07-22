using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Vostok.Metrics;

namespace Vostok.Extensions.Hosting.Components.Metrics;

[PublicAPI]
public interface IVostokMetricsBuilder
{
    IServiceCollection Services { get; }

    IVostokMetricsBuilder Configure(Action<MetricContextConfig, IServiceProvider> configure);
    IVostokMetricsBuilder AddMetricSender(Func<IServiceProvider, IMetricEventSender> implementationFactory);
    IVostokMetricsBuilder ClearMetricSenders();
    IVostokMetricsBuilder CustomizeAnnotationEventSender(Func<IAnnotationEventSender, IServiceProvider, IAnnotationEventSender> senderCustomization);
}