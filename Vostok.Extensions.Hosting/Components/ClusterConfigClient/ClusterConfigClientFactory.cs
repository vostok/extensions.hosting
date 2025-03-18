using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.ClusterConfig.Client;
using Vostok.ClusterConfig.Client.Abstractions;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ClusterConfigClient;

internal static class ClusterConfigClientFactory
{
    public static IClusterConfigClient Create(IServiceProvider provider)
    {
        var log = provider.GetRequiredService<ILog>();
        var settings = provider.GetRequiredService<IOptions<ClusterConfigClientSettings>>().Value;

        if (settings is not { EnableLocalSettings: false, EnableClusterSettings: false })
            return new ClusterConfig.Client.ClusterConfigClient(settings);

        log.LogDisabled("ClusterConfigClient", "explicit configuration");
        return new DisabledClusterConfigClient();
    }
}