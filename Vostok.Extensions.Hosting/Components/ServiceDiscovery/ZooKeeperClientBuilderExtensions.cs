using JetBrains.Annotations;

using Vostok.Clusterclient.Core.Topology;
using Vostok.Extensions.Hosting.Helpers;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

[PublicAPI]
public static class ZooKeeperClientBuilderExtensions
{
    /// <summary>
    /// Sets provided <see cref="IClusterProvider"/> for ZooKeeper topology.
    /// </summary>
    /// <param name="builder">The <see cref="IZooKeeperClientBuilder"/></param>
    /// <param name="clusterProvider">The <see cref="IClusterProvider"/>.</param>
    /// <returns><see cref="IZooKeeperClientBuilder"/> for chain method calls.</returns>
    /// <remarks>This method will clear registered connection string.</remarks>
    public static IZooKeeperClientBuilder SetClusterProvider(
        this IZooKeeperClientBuilder builder,
        IClusterProvider clusterProvider) =>
        builder.SetClusterProvider(_ => clusterProvider);

    public static IZooKeeperClientBuilder SetClusterConfigTopology(this IZooKeeperClientBuilder builder, string path) =>
        builder.SetClusterProvider(provider => ClusterProviderFactory.FromClusterConfig(provider, path));
}