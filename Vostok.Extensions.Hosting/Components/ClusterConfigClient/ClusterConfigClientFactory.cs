using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.ClusterConfig.Client;
using Vostok.ClusterConfig.Client.Abstractions;
using Vostok.Extensions.Hosting.Helpers;

namespace Vostok.Extensions.Hosting.Components.ClusterConfigClient;

internal static class ClusterConfigClientFactory
{
    public static IClusterConfigClient Create(IServiceProvider provider)
    {
        var log = provider.GetInitializationLog();
        var settings = provider.GetRequiredService<IOptions<ClusterConfigClientSettings>>().Value;

        if (settings is not { EnableLocalSettings: false, EnableClusterSettings: false })
            return new ClusterConfig.Client.ClusterConfigClient(settings);

        log.LogDisabled("ClusterConfigClient");
        return new ClusterConfig.Client.ClusterConfigClient(new ClusterConfigClientSettings
        {
            EnableLocalSettings = false,
            EnableClusterSettings = false
        });
    }
}