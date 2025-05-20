using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Components.ServiceDiscovery;
using Vostok.Extensions.Hosting.Components.ServiceDiscovery.EventsContext;
using Vostok.Extensions.Hosting.Components.ZooKeeperClient;
using Vostok.ServiceDiscovery;
using Vostok.ZooKeeper.Client;
using Vostok.ZooKeeper.Client.Abstractions;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class ServiceBeaconBuilderExtensions
{
    public static IVostokBuilder WithServiceBeacon(this IVostokBuilder builder, Action<IServiceBeaconBuilder> configure)
    {
        builder.ConfigureServices(services =>
        {
            services.AddServiceBeacon();

            var serviceBeaconBuilder = new ServiceBeaconBuilder(services);
            configure(serviceBeaconBuilder);
        });

        return builder;
    }

    public static IVostokBuilder DisableServiceBeacon(this IVostokBuilder builder)
    {
        builder.ConfigureServices(services =>
            services.Replace(ServiceDescriptor.Singleton(ServiceBeaconFactory.CreateDisabled)));

        return builder;
    }

    public static IVostokBuilder WithServiceLocator(
        this IVostokBuilder builder,
        Action<ServiceLocatorSettings> configure)
    {
        builder.WithServiceLocator((settings, _) => configure(settings));

        return builder;
    }

    public static IVostokBuilder WithServiceLocator(
        this IVostokBuilder builder,
        Action<ServiceLocatorSettings, IServiceProvider>? configure = null)
    {
        builder.ConfigureServices(services =>
        {
            services.AddServiceLocator();

            if (configure is not null)
                services.AddOptions<ServiceLocatorSettings>().Configure(configure);
        });

        return builder;
    }

    public static IVostokBuilder WithZooKeeperClient(
        this IVostokBuilder builder,
        Action<IZooKeeperClientBuilder> configure)
    {
        builder.ConfigureServices(services =>
        {
            services.AddZooKeeperClient();

            var zooKeeperClientBuilder = new ZooKeeperClientBuilder(services);
            configure(zooKeeperClientBuilder);
        });

        return builder;
    }

    public static IVostokBuilder DisableZooKeeper(this IVostokBuilder builder)
    {
        builder.ConfigureServices(services =>
            services.Replace(ServiceDescriptor.Singleton<IZooKeeperClient>(new DevNullZooKeeperClient())));

        return builder;
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