using Microsoft.Extensions.Options;

using Vostok.Metrics;
using Vostok.Metrics.Senders;

namespace Vostok.Extensions.Hosting.Components.Metrics.Options;

internal sealed class MetricContextConfigFactory(
    IEnumerable<IConfigureOptions<MetricContextConfig>> setups,
    IEnumerable<IPostConfigureOptions<MetricContextConfig>> postConfigures,
    IEnumerable<IValidateOptions<MetricContextConfig>> validations,
    IEnumerable<IMetricEventSender> senders)
    : OptionsFactory<MetricContextConfig>(setups, postConfigures, validations)
{
    protected override MetricContextConfig CreateInstance(string _)
    {
        var compositeMetricEventSender = BuildCompositeMetricEventSender(senders.ToArray());
        return new MetricContextConfig(compositeMetricEventSender ?? new DevNullMetricEventSender());
    }

    private static IMetricEventSender? BuildCompositeMetricEventSender(IMetricEventSender[] senders) =>
        senders switch
        {
            [] => null,
            [var sender] => sender,
            _ => new CompositeMetricEventSender(senders)
        };
}