using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.Components.ZooKeeperClient;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class ZooKeeperClientBuilderExtensions
{
    public static IVostokBuilder WithZooKeeperClient(
        this IVostokBuilder builder,
        Action<ZooKeeperClientConfigurationOptions> configure)
    {
        builder.WithZooKeeperClient((options, _) => configure(options));

        return builder;
    }

    public static IVostokBuilder WithZooKeeperClient(
        this IVostokBuilder builder,
        Action<ZooKeeperClientConfigurationOptions, IServiceProvider>? configure = null)
    {
        builder.Services.AddZooKeeperClient();

        if (configure is not null)
            builder.Services.AddOptions<ZooKeeperClientConfigurationOptions>().Configure(configure);

        return builder;
    }

    private static void AddZooKeeperClient(this IServiceCollection services) =>
        services.TryAddSingleton(ZooKeeperClientFactory.Create);
}