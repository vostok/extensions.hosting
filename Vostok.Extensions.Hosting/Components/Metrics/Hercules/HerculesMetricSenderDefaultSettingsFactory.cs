using Microsoft.Extensions.Options;

using Vostok.Hercules.Client.Abstractions;
using Vostok.Metrics.Hercules;

namespace Vostok.Extensions.Hosting.Components.Metrics.Hercules;

internal sealed class HerculesMetricSenderDefaultSettingsFactory(
    IEnumerable<IConfigureOptions<HerculesMetricSenderSettings>> setups,
    IEnumerable<IPostConfigureOptions<HerculesMetricSenderSettings>> postConfigures,
    IEnumerable<IValidateOptions<HerculesMetricSenderSettings>> validations,
    IHerculesSink sink) : OptionsFactory<HerculesMetricSenderSettings>(setups, postConfigures, validations)
{
    protected override HerculesMetricSenderSettings CreateInstance(string _) => new(sink);
}