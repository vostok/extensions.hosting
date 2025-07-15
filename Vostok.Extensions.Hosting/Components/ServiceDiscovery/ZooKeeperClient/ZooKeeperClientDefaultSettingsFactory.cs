using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.ZooKeeper.Client;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery.ZooKeeperClient;

internal sealed class ZooKeeperClientDefaultSettingsFactory(
    IEnumerable<IConfigureOptions<ZooKeeperClientSettings>> setups,
    IEnumerable<IPostConfigureOptions<ZooKeeperClientSettings>> postConfigures,
    IEnumerable<IValidateOptions<ZooKeeperClientSettings>> validations,
    IServiceProvider provider) : OptionsFactory<ZooKeeperClientSettings>(setups, postConfigures, validations)
{
    protected override ZooKeeperClientSettings CreateInstance(string name)
    {
        var miscOptions = provider.GetRequiredService<IOptions<ZooKeeperClientMiscellaneousOptions>>().Value;
        if (miscOptions.ConnectionString is not null)
            return new ZooKeeperClientSettings(miscOptions.ConnectionString);

        var clusterProvider = miscOptions.ClusterProvider?.Invoke(provider);
        if (clusterProvider is not null)
            return new ZooKeeperClientSettings(() => clusterProvider.GetCluster());

        throw new InvalidOperationException("Unconfigured topology");
    }
}