using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Vostok.Clusterclient.Core.Topology;
using Vostok.ZooKeeper.Client;
using Vostok.ZooKeeper.Client.Abstractions.Model.Authentication;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

[PublicAPI]
public interface IZooKeeperClientBuilder
{
    IServiceCollection Services { get; }

    IZooKeeperClientBuilder Configure(Action<ZooKeeperClientSettings, IServiceProvider> configure);
    IZooKeeperClientBuilder SetConnectionString(string connectionString);
    IZooKeeperClientBuilder SetClusterProvider(Func<IServiceProvider, IClusterProvider?> implementationFactory);
    IZooKeeperClientBuilder AddAuthenticationInfo(AuthenticationInfo authenticationInfo);
}