using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Vostok.Clusterclient.Core.Topology;
using Vostok.ZooKeeper.Client;
using Vostok.ZooKeeper.Client.Abstractions;
using Vostok.ZooKeeper.Client.Abstractions.Model.Authentication;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

/// <summary>
/// Builder for configuring <see cref="IZooKeeperClient"/>.
/// </summary>
[PublicAPI]
public interface IZooKeeperClientBuilder
{
    /// <summary>
    /// The host <see cref="IServiceCollection"/> where <see cref="IZooKeeperClient"/> is configured.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Configures <see cref="ZooKeeperClientSettings"/>.
    /// </summary>
    /// <param name="configure">Delegate to configure <see cref="ZooKeeperClientSettings"/>.</param>
    /// <returns><see cref="IZooKeeperClientBuilder"/> for chain method calls.</returns>
    IZooKeeperClientBuilder Configure(Action<ZooKeeperClientSettings> configure);

    /// <summary>
    /// Sets provided connection string.
    /// </summary>
    /// <param name="connectionString">The ZooKeeper connection string.</param>
    /// <returns><see cref="IZooKeeperClientBuilder"/> for chain method calls.</returns>
    /// <remarks>This method will clear registered <see cref="IClusterProvider"/>.</remarks>
    IZooKeeperClientBuilder SetConnectionString(string connectionString);

    /// <summary>
    /// Sets provided <see cref="IClusterProvider"/> for ZooKeeper topology.
    /// </summary>
    /// <param name="implementationFactory">The <see cref="IClusterProvider"/> implementation factory.</param>
    /// <returns><see cref="IZooKeeperClientBuilder"/> for chain method calls.</returns>
    /// <remarks>This method will clear registered connection string.</remarks>
    IZooKeeperClientBuilder SetClusterProvider(Func<IServiceProvider, IClusterProvider?> implementationFactory);

    /// <summary>
    /// Adds <see cref="AuthenticationInfo"/> for <see cref="IZooKeeperClient"/>.
    /// </summary>
    /// <param name="authenticationInfo">The <see cref="AuthenticationInfo"/>.</param>
    /// <returns><see cref="IZooKeeperClientBuilder"/> for chain method calls.</returns>
    IZooKeeperClientBuilder AddAuthenticationInfo(AuthenticationInfo authenticationInfo);
}