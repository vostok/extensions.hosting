using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Logging.Abstractions;
using Vostok.ServiceDiscovery;
using Vostok.ZooKeeper.Client;
using Vostok.ZooKeeper.Client.Abstractions;
using Vostok.ZooKeeper.Client.Abstractions.Model;

namespace Vostok.Extensions.Hosting.Components.ZooKeeperClient;

internal static class ZooKeeperClientFactory
{
    public static IZooKeeperClient Create(IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<ZooKeeperClientConfigurationOptions>>().Value;
        var log = provider.GetRequiredService<ILog>();

        ZooKeeperClientSettings? settings = null;

        // todo: Enable/Disable

        if (options.ConnectionString is not null)
            settings = new ZooKeeperClientSettings(options.ConnectionString);

        var clusterProvider = options.ClusterProviderBuilder?.Build(provider);
        if (clusterProvider is not null)
            settings = new ZooKeeperClientSettings(() => clusterProvider.GetCluster());

        if (settings is null)
        {
            log.LogDisabled("ZooKeeperClient", "not configured ZooKeeper topology");
            return new DevNullZooKeeperClient();
        }

        options.ApplyCustomizations(settings);

        var zooKeeperClient = new ZooKeeper.Client.ZooKeeperClient(
            settings,
            log.WithEventsDroppedByProperties(IsDataChangedLog));

        options.ApplyAuthenticationInfos(zooKeeperClient);

        // todo: StaticProvider configurator
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