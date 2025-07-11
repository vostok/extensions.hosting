using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Components.ServiceDiscovery.ServiceBeacon;
using Vostok.Extensions.Hosting.Components.ServiceDiscovery.ServiceBeacon.EventsContext;
using Vostok.Extensions.Hosting.Components.ServiceDiscovery.ServiceLocator;
using Vostok.Extensions.Hosting.Components.ServiceDiscovery.ZooKeeperClient;
using Vostok.ServiceDiscovery;
using Vostok.ServiceDiscovery.Abstractions;
using Vostok.ZooKeeper.Client;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

[PublicAPI]
public static class VostokBuilderServiceDiscoveryExtensions
{
    public static IVostokBuilder WithServiceBeacon(this IVostokBuilder vostok, Action<IServiceBeaconBuilder> configure)
    {
        if (!vostok.CheckComponentAlreadyAdded("ServiceBeacon"))
            vostok.Services.AddServiceBeacon();

        var serviceBeaconBuilder = new ServiceBeaconBuilder(vostok.Services);
        configure(serviceBeaconBuilder);

        return vostok;
    }

    /// <summary>
    /// Disables <see cref="IServiceBeacon"/> and forces it not to start.
    /// </summary>
    /// <remarks>
    /// Configuration done with <see cref="IServiceBeaconBuilder"/> will still be applied and <see cref="IReplicaInfo"/> can still be consumed from disabled beacon.
    /// </remarks>
    public static IVostokBuilder DisableServiceBeacon(this IVostokBuilder vostok)
    {
        vostok.Services.Replace(ServiceDescriptor.Singleton(ServiceBeaconFactory.CreateDisabled));
        return vostok;
    }

    public static IVostokBuilder WithServiceLocator(
        this IVostokBuilder vostok,
        Action<ServiceLocatorSettings> configure)
    {
        vostok.WithServiceLocator((settings, _) => configure(settings));

        return vostok;
    }

    public static IVostokBuilder WithServiceLocator(
        this IVostokBuilder vostok,
        Action<ServiceLocatorSettings, IServiceProvider>? configure = null)
    {
        if (!vostok.CheckComponentAlreadyAdded("ServiceLocator"))
            vostok.Services.AddServiceLocator();

        if (configure is not null)
            vostok.Services.AddOptions<ServiceLocatorSettings>().Configure(configure);

        return vostok;
    }

    public static IVostokBuilder WithZooKeeperClient(
        this IVostokBuilder vostok,
        Action<IZooKeeperClientBuilder> configure)
    {
        if (!vostok.CheckComponentAlreadyAdded("ZooKeeperClient"))
            vostok.Services.AddZooKeeperClient();

        var zooKeeperClientBuilder = new ZooKeeperClientBuilder(vostok.Services);
        configure(zooKeeperClientBuilder);

        return vostok;
    }

    public static IVostokBuilder DisableZooKeeper(this IVostokBuilder vostok)
    {
        vostok.Services.Replace(ServiceDescriptor.Singleton(ZooKeeperClientFactory.CreateDisabled));
        return vostok;
    }

    private static void AddServiceBeacon(this IServiceCollection services)
    {
        services.TryAddSingleton(ServiceBeaconFactory.Create);
        services.TryAddSingleton(ServiceDiscoveryEventsContextFactory.Create);
        services.AddHostedService<ServiceBeaconHostedService>();
    }

    private static void AddServiceLocator(this IServiceCollection services) =>
        services.TryAddSingleton(ServiceLocatorFactory.Create);

    private static void AddZooKeeperClient(this IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IOptionsFactory<ZooKeeperClientSettings>, ZooKeeperClientDefaultSettingsFactory>());
        services.TryAddSingleton(ZooKeeperClientFactory.Create);
    }
}