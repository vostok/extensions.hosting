using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Metrics;

namespace Vostok.Extensions.Hosting.Components.Metrics;

internal sealed class VostokMetricsBuilder(IServiceCollection services) : IVostokMetricsBuilder
{
    public IServiceCollection Services { get; } = services;

    public IVostokMetricsBuilder Configure(Action<MetricContextConfig, IServiceProvider> configure)
    {
        Services.AddOptions<MetricContextConfig>().Configure(configure);
        return this;
    }

    public IVostokMetricsBuilder AddMetricSender(Func<IServiceProvider, IMetricEventSender> implementationFactory)
    {
        Services.AddSingleton(implementationFactory);
        return this;
    }

    public IVostokMetricsBuilder ClearMetricSenders()
    {
        Services.RemoveAll<IMetricEventSender>();
        return this;
    }

    public IVostokMetricsBuilder CustomizeAnnotationEventSender(
        Func<IAnnotationEventSender, IServiceProvider, IAnnotationEventSender> senderCustomization)
    {
        Services.Configure<VostokMetricsMiscellaneousOptions>(options =>
            options.AnnotationsSenderCustomizations.Add(senderCustomization));

        return this;
    }
}