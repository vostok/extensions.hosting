using Vostok.ClusterConfig.Client;

namespace Vostok.Extensions.Hosting.Components.ClusterConfigClient;

internal sealed class DisabledClusterConfigClient : ClusterConfig.Client.ClusterConfigClient
{
    public DisabledClusterConfigClient()
        : base(
            new ClusterConfigClientSettings
            {
                EnableLocalSettings = false,
                EnableClusterSettings = false
            })
    {
    }
}