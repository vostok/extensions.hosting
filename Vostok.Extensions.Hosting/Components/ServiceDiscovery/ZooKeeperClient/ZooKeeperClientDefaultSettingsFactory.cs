using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Clusterclient.Core.Topology;
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
        var connectionStringOptions = provider.GetRequiredService<IOptions<ZooKeeperClientConnectionStringOptions>>();
        var connectionString = connectionStringOptions.Value.ConnectionString;
        if (connectionString is not null)
            return new ZooKeeperClientSettings(connectionString);

        var clusterProvider = provider.GetKeyedService<IClusterProvider>(ZooKeeperClientFactory.ClusterProviderKey);
        if (clusterProvider is not null)
            return new ZooKeeperClientSettings(() => clusterProvider.GetCluster());

        throw new InvalidOperationException("Unconfigured topology");
    }
}