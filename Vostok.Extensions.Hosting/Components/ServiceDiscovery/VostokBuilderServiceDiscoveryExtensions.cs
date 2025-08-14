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
using Vostok.ZooKeeper.Client.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

[PublicAPI]
public static class VostokBuilderServiceDiscoveryExtensions
{
    /// <summary>
    /// Registers and configures services for <see cref="IServiceBeacon"/>.
    /// </summary>
    /// <param name="vostok">The <see cref="IVostokBuilder"/>.</param>
    /// <param name="configure">Delegate to configure <see cref="IServiceBeaconBuilder"/>.</param>
    /// <returns><see cref="IVostokBuilder"/> for chain method calls.</returns>
    /// <remarks>Calling this method multiple times on the same <see cref="IVostokBuilder"/> instance will result in one <see cref="IServiceBeacon"/> configured.</remarks>
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

    /// <summary>
    /// Registers and configures services for <see cref="IServiceLocator"/>.
    /// </summary>
    /// <param name="vostok">The <see cref="IVostokBuilder"/>.</param>
    /// <param name="configure">Delegate to configure <see cref="ServiceLocatorSettings"/>.</param>
    /// <returns><see cref="IVostokBuilder"/> for chain method calls.</returns>
    /// <remarks>Calling this method multiple times on the same <see cref="IVostokBuilder"/> instance will result in one <see cref="IServiceLocator"/> configured.</remarks>
    public static IVostokBuilder WithServiceLocator(
        this IVostokBuilder vostok,
        Action<ServiceLocatorSettings>? configure = null)
    {
        if (!vostok.CheckComponentAlreadyAdded("ServiceLocator"))
            vostok.Services.AddServiceLocator();

        if (configure is not null)
            vostok.Services.Configure(configure);

        return vostok;
    }

    /// <summary>
    /// Registers and configures services for <see cref="IZooKeeperClient"/>.
    /// </summary>
    /// <param name="vostok">The <see cref="IVostokBuilder"/>.</param>
    /// <param name="configure">Delegate to configure <see cref="IZooKeeperClientBuilder"/>.</param>
    /// <returns><see cref="IVostokBuilder"/> for chain method calls.</returns>
    /// <remarks>Calling this method multiple times on the same <see cref="IVostokBuilder"/> instance will result in one <see cref="IZooKeeperClient"/> configured.</remarks>
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