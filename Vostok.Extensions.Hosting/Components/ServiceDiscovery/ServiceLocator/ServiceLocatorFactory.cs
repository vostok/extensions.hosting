using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Components.ServiceDiscovery.ZooKeeperClient;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.ServiceDiscovery;
using Vostok.ServiceDiscovery.Abstractions;
using Vostok.ZooKeeper.Client.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery.ServiceLocator;

internal static class ServiceLocatorFactory
{
    public static IServiceLocator Create(IServiceProvider provider)
    {
        var settings = provider.GetRequiredService<IOptions<ServiceLocatorSettings>>().Value;
        var zooKeeperClient = provider.GetService<IZooKeeperClient>();
        var log = provider.GetInitializationLog();

        if (zooKeeperClient is null or DevNullZooKeeperClient)
        {
            log.LogDisabled("ServiceLocator", "disabled ZooKeeperClient");
            return new DevNullServiceLocator();
        }

        return new Vostok.ServiceDiscovery.ServiceLocator(zooKeeperClient, settings, log);
    }
}