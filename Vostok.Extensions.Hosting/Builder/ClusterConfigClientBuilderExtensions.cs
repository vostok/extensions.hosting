using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.Components.ClusterConfigClient;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class ClusterConfigClientBuilderExtensions
{
    public static IVostokBuilder WithClusterConfigClient(
        this IVostokBuilder builder,
        Action<ClusterConfigClientConfigurationOptions> configure)
    {
        builder.WithClusterConfigClient((options, _) => configure(options));

        return builder;
    }

    public static IVostokBuilder WithClusterConfigClient(
        this IVostokBuilder builder,
        Action<ClusterConfigClientConfigurationOptions, IServiceProvider>? configure = null)
    {
        builder.Services.AddClusterConfigClient();

        if (configure is not null)
            builder.Services.AddOptions<ClusterConfigClientConfigurationOptions>().Configure(configure);

        return builder;
    }

    private static void AddClusterConfigClient(this IServiceCollection services) =>
        services.TryAddSingleton(ClusterConfigClientFactory.Create);
}