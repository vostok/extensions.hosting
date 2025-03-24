using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Components.ZooKeeperClient;
using Vostok.ZooKeeper.Client;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class ZooKeeperClientBuilderExtensions
{
    public static IVostokBuilder WithZooKeeperClient(
        this IVostokBuilder builder,
        Action<IZooKeeperClientBuilder> configure)
    {
        builder.Services.AddZooKeeperClient();

        var zooKeeperClientBuilder = new ZooKeeperClientBuilder(builder.Services);
        configure(zooKeeperClientBuilder);

        return builder;
    }

    private static void AddZooKeeperClient(this IServiceCollection services)
    {
        services.TryAddSingleton<IOptionsFactory<ZooKeeperClientSettings>, ZooKeeperClientSettingsFactory>();
        services.TryAddSingleton(ZooKeeperClientFactory.Create);
    }
}