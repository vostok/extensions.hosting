using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Logging.Abstractions;
using Vostok.ServiceDiscovery;
using Vostok.ZooKeeper.Client;
using Vostok.ZooKeeper.Client.Abstractions;
using Vostok.ZooKeeper.Client.Abstractions.Model;
using Vostok.ZooKeeper.Client.Abstractions.Model.Authentication;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery.ZooKeeperClient;

internal static class ZooKeeperClientFactory
{
    public const string ClusterProviderKey = "ZooKeeperClientClusterProvider";

    public static IZooKeeperClient Create(IServiceProvider provider)
    {
        var log = provider.GetInitializationLog();
        var settings = provider.GetRequiredService<IOptions<ZooKeeperClientSettings>>().Value;
        var authenticationInfos = provider.GetServices<AuthenticationInfo>();

        var zooKeeperClient = new ZooKeeper.Client.ZooKeeperClient(
            settings,
            log.WithEventsDroppedByProperties(IsDataChangedLog));

        foreach (var authenticationInfo in authenticationInfos)
            zooKeeperClient.AddAuthenticationInfo(authenticationInfo);

        return zooKeeperClient;
    }

    private static bool IsDataChangedLog(IReadOnlyDictionary<string, object> properties)
    {
        if (!properties.TryGetValue("NodeEventType", out var eventType))
            return false;
        if (!properties.TryGetValue("NodePath", out var path))
            return false;
        return
            eventType is NodeChangedEventType castedEventType && castedEventType == NodeChangedEventType.DataChanged &&
            path is string castedPath && castedPath.StartsWith(new ServiceLocatorSettings().ZooKeeperNodesPrefix);
    }
}