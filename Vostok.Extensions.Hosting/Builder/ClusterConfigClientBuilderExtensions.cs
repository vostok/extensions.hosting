using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.ClusterConfig.Client;
using Vostok.Extensions.Hosting.Components.ClusterConfigClient;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class ClusterConfigClientBuilderExtensions
{
    public static IVostokBuilder WithClusterConfigClient(
        this IVostokBuilder builder,
        Action<ClusterConfigClientSettings> configure)
    {
        builder.WithClusterConfigClient((options, _) => configure(options));

        return builder;
    }

    public static IVostokBuilder WithClusterConfigClient(
        this IVostokBuilder builder,
        Action<ClusterConfigClientSettings, IServiceProvider>? configure = null)
    {
        builder.Services.AddClusterConfigClient();

        if (configure is not null)
            builder.Services.AddOptions<ClusterConfigClientSettings>().Configure(configure);

        return builder;
    }

    private static void AddClusterConfigClient(this IServiceCollection services)
    {
        services.TryAddSingleton<IConfigureOptions<ClusterConfigClientSettings>, ClusterConfigClientDefaultConfiguration>();
        services.TryAddSingleton(ClusterConfigClientFactory.Create);
    }
}