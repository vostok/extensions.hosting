using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.ClusterConfig.Client;
using Vostok.ClusterConfig.Client.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ClusterConfigClient;

[PublicAPI]
public static class VostokBuilderClusterConfigClientExtensions
{
    public static IVostokBuilder WithClusterConfigClient(
        this IVostokBuilder vostok,
        Action<ClusterConfigClientSettings>? configure = null)
    {
        if (!vostok.CheckAndMarkComponentAdded("ClusterConfigClient"))
            vostok.Services.AddClusterConfigClient();

        if (configure is not null)
            vostok.Services.Configure(configure);

        return vostok;
    }

    /// <summary>
    /// Disables <see cref="IClusterConfigClient"/> altogether.
    /// </summary>
    public static IVostokBuilder DisableClusterConfig(this IVostokBuilder vostok) =>
        vostok.WithClusterConfigClient(settings =>
        {
            settings.EnableLocalSettings = false;
            settings.EnableClusterSettings = false;
        });

    /// <summary>
    /// Enables <see cref="IClusterConfigClient"/> in case it was disabled.
    /// </summary>
    public static IVostokBuilder EnableClusterConfig(this IVostokBuilder vostok) =>
        vostok.WithClusterConfigClient(settings =>
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