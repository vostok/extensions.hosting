using Vostok.ZooKeeper.Client;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery.ZooKeeperClient;

internal sealed class DevNullZookeeperClientSettings() : ZooKeeperClientSettings("_");