using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Clusterclient.Core.Topology;
using Vostok.Extensions.Hosting.Components.ServiceDiscovery.ZooKeeperClient;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.ZooKeeper.Client;
using Vostok.ZooKeeper.Client.Abstractions.Model.Authentication;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

[PublicAPI]
public static class ZooKeeperClientBuilderExtensions
{
    public static IZooKeeperClientBuilder Configure(
        this IZooKeeperClientBuilder builder,
        Action<ZooKeeperClientSettings> configure)
    {
        builder.Configure((settings, _) => configure(settings));
        return builder;
    }

    public static IZooKeeperClientBuilder Configure(
        this IZooKeeperClientBuilder builder,
        Action<ZooKeeperClientSettings, IServiceProvider>? configure = null)
    {
        if (configure is not null)
            builder.Services.AddOptions<ZooKeeperClientSettings>().Configure(configure);
        return builder;
    }

    public static IZooKeeperClientBuilder SetClusterProvider(
        this IZooKeeperClientBuilder builder,
        IClusterProvider clusterProvider)
    {
        builder.SetClusterProvider(_ => clusterProvider);
        return builder;
    }

    public static IZooKeeperClientBuilder SetClusterProvider(
        this IZooKeeperClientBuilder builder,
        Func<IServiceProvider, IClusterProvider> implementationFactory)
    {
        builder.Services.AddKeyedSingleton(
            ZooKeeperClientFactory.ClusterProviderKey,
            (provider, _) => implementationFactory(provider));
        builder.Services.Configure<ZooKeeperClientConnectionStringOptions>(o => o.ConnectionString = null);

        return builder;
    }

    public static IZooKeeperClientBuilder SetConnectionString(
        this IZooKeeperClientBuilder builder,
        string connectionString)
    {
        builder.Services.Configure<ZooKeeperClientConnectionStringOptions>(o => o.ConnectionString = connectionString);
        builder.Services.RemoveAllKeyed<IClusterProvider>(ZooKeeperClientFactory.ClusterProviderKey);
        return builder;
    }

    public static IZooKeeperClientBuilder SetClusterConfigTopology(this IZooKeeperClientBuilder builder, string path) =>
        builder.SetClusterProvider(provider => ClusterProviderFactory.FromClusterConfig(provider, path));

    public static IZooKeeperClientBuilder AddAuthenticationInfo(
        this IZooKeeperClientBuilder builder,
        AuthenticationInfo authenticationInfo)
    {
        builder.Services.AddSingleton(authenticationInfo);
        return builder;
    }
}