using Microsoft.Extensions.DependencyInjection;

using Vostok.Clusterclient.Core.Topology;
using Vostok.ZooKeeper.Client;
using Vostok.ZooKeeper.Client.Abstractions.Model.Authentication;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery.ZooKeeperClient;

internal sealed class ZooKeeperClientBuilder(IServiceCollection services) : IZooKeeperClientBuilder
{
    public IServiceCollection Services { get; } = services;

    public IZooKeeperClientBuilder Configure(Action<ZooKeeperClientSettings, IServiceProvider> configure)
    {
        Services.AddOptions<ZooKeeperClientSettings>().Configure(configure);
        return this;
    }

    public IZooKeeperClientBuilder SetConnectionString(string connectionString)
    {
        ConfigureMisc(options =>
        {
            options.ConnectionString = connectionString;
            options.ClusterProvider = null;
        });

        return this;
    }

    public IZooKeeperClientBuilder SetClusterProvider(Func<IServiceProvider, IClusterProvider?> implementationFactory)
    {
        ConfigureMisc(options =>
        {
            options.ConnectionString = null;
            options.ClusterProvider = implementationFactory;
        });

        return this;
    }

    public IZooKeeperClientBuilder AddAuthenticationInfo(AuthenticationInfo authenticationInfo)
    {
        ConfigureMisc(options => options.AuthenticationInfos.Add(authenticationInfo));
        return this;
    }

    private void ConfigureMisc(Action<ZooKeeperClientMiscellaneousOptions> configure) =>
        Services.Configure(configure);
}