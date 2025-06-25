using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.ClusterConfig.Client;
using Vostok.ClusterConfig.Client.Abstractions;
using Vostok.Extensions.Hosting.Components.ClusterConfigClient;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class VostokBuilderClusterConfigClientExtensions
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
        if (!builder.CheckComponentAlreadyAdded("ClusterConfigClient"))
            builder.Services.AddClusterConfigClient();

        if (configure is not null)
            builder.Services.AddOptions<ClusterConfigClientSettings>().Configure(configure);

        return builder;
    }

    /// <summary>
    /// Disables <see cref="IClusterConfigClient"/> altogether.
    /// </summary>
    public static IVostokBuilder DisableClusterConfig(this IVostokBuilder builder) =>
        builder.WithClusterConfigClient(settings =>
        {
            settings.EnableLocalSettings = false;
            settings.EnableClusterSettings = false;
        });

    /// <summary>
    /// Enables <see cref="IClusterConfigClient"/> in case it was disabled.
    /// </summary>
    public static IVostokBuilder EnableClusterConfig(this IVostokBuilder builder) =>
        builder.WithClusterConfigClient(settings =>
        {
            settings.EnableLocalSettings = true;
            settings.EnableClusterSettings = true;
        });

    private static void AddClusterConfigClient(this IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IOptionsFactory<ClusterConfigClientSettings>, ClusterConfigClientDefaultSettingsFactory>());
        services.TryAddSingleton(ClusterConfigClientFactory.Create);
    }
}