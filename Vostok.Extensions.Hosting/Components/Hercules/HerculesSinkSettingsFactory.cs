using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Clusterclient.Core.Topology;
using Vostok.Hercules.Client;

namespace Vostok.Extensions.Hosting.Components.Hercules;

internal sealed class HerculesSinkSettingsFactory(
    IEnumerable<IConfigureOptions<HerculesSinkSettings>> setups,
    IEnumerable<IPostConfigureOptions<HerculesSinkSettings>> postConfigures,
    IEnumerable<IValidateOptions<HerculesSinkSettings>> validations,
    IServiceProvider provider)
    : OptionsFactory<HerculesSinkSettings>(setups, postConfigures, validations)
{
    protected override HerculesSinkSettings CreateInstance(string _)
    {
        var clusterProvider = provider.GetRequiredKeyedService<IClusterProvider>(HerculesSinkFactory.ClusterProviderKey);
        var optionsMonitor = provider.GetRequiredService<IOptionsMonitor<HerculesSinkMiscellaneousOptions>>();

        return new HerculesSinkSettings(clusterProvider, () => optionsMonitor.CurrentValue.DefaultApiKey);
    }
}