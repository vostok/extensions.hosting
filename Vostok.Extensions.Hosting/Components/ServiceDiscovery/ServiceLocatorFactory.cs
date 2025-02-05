using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Components.ZooKeeperClient;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.Logging.Abstractions;
using Vostok.ServiceDiscovery;
using Vostok.ServiceDiscovery.Abstractions;
using Vostok.ZooKeeper.Client.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

internal static class ServiceLocatorFactory
{
    public static IServiceLocator Create(IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<ServiceLocatorConfigurationOptions>>().Value;
        var log = provider.GetRequiredService<ILog>();
        var zooKeeperClient = provider.GetService<IZooKeeperClient>();

        if (zooKeeperClient is null or DevNullZooKeeperClient)
        {
            log.LogDisabled("ServiceLocator", "disabled ZooKeeperClient");
            return new DevNullServiceLocator();
        }

        var settings = new ServiceLocatorSettings();

        options.ApplyCustomizations(settings);

        return new ServiceLocator(zooKeeperClient, settings);
    }
}