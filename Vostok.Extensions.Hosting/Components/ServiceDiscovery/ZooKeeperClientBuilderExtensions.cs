using JetBrains.Annotations;

using Vostok.Clusterclient.Core.Topology;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.ZooKeeper.Client;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

[PublicAPI]
public static class ZooKeeperClientBuilderExtensions
{
    public static IZooKeeperClientBuilder Configure(
        this IZooKeeperClientBuilder builder,
        Action<ZooKeeperClientSettings> configure) =>
        builder.Configure((settings, _) => configure(settings));

    public static IZooKeeperClientBuilder SetClusterProvider(
        this IZooKeeperClientBuilder builder,
        IClusterProvider clusterProvider) =>
        builder.SetClusterProvider(_ => clusterProvider);

    public static IZooKeeperClientBuilder SetClusterConfigTopology(this IZooKeeperClientBuilder builder, string path) =>
        builder.SetClusterProvider(provider => ClusterProviderFactory.FromClusterConfig(provider, path));
}