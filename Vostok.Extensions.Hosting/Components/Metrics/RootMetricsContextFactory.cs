using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Metrics;
using Vostok.Metrics.Senders;

namespace Vostok.Extensions.Hosting.Components.Metrics;

internal static class RootMetricsContextFactory
{
    public static IMetricContext Create(IServiceProvider provider)
    {
        var config = provider.GetRequiredService<IOptions<MetricContextConfig>>().Value;

        if (config.Sender is DevNullMetricEventSender)
        {
            provider.GetInitializationLog().LogDisabled("MetricsContext", "unconfigured sender");
            return new DevNullMetricContext();
        }

        return new MetricContext(config);
    }
}