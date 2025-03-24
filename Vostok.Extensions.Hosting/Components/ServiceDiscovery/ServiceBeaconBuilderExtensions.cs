using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Vostok.Datacenters;
using Vostok.Extensions.Hosting.Components.ServiceDiscovery.EventsContext;
using Vostok.ServiceDiscovery;
using Vostok.ServiceDiscovery.Telemetry.EventsSender;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

[PublicAPI]
public static class ServiceBeaconBuilderExtensions
{
    public static IServiceBeaconBuilder Configure(
        this IServiceBeaconBuilder builder,
        Action<ServiceBeaconSettings> configure)
    {
        builder.Configure((settings, _) => configure(settings));
        return builder;
    }

    public static IServiceBeaconBuilder Configure(
        this IServiceBeaconBuilder builder,
        Action<ServiceBeaconSettings, IServiceProvider>? configure = null)
    {
        if (configure is not null)
            builder.Services.AddOptions<ServiceBeaconSettings>().Configure(configure);
        return builder;
    }

    public static IServiceBeaconBuilder CustomizeReplicaInfo(
        this IServiceBeaconBuilder builder,
        ReplicaInfoSetup configure)
    {
        builder.Services.AddSingleton(configure);
        return builder;
    }

    public static IServiceBeaconBuilder CustomizeReplicaInfo(
        this IServiceBeaconBuilder builder,
        Func<IServiceProvider, ReplicaInfoSetup> implementationFactory)
    {
        builder.Services.AddSingleton(implementationFactory);
        return builder;
    }

    public static IServiceBeaconBuilder DenyRegistrationFromNonActiveDatacenters(this IServiceBeaconBuilder builder)
    {
        builder.Configure((settings, provider) =>
        {
            if (provider.GetService<IDatacenters>() is { } datacenters)
                settings.RegistrationAllowedProvider = datacenters.LocalDatacenterIsActive;
        });

        return builder;
    }

    public static IServiceBeaconBuilder SetEventSender(
        this IServiceBeaconBuilder builder,
        IServiceDiscoveryEventsSender eventsSender)
    {
        builder.SetEventSender(_ => eventsSender);
        return builder;
    }

    public static IServiceBeaconBuilder SetEventSender(
        this IServiceBeaconBuilder builder,
        Func<IServiceProvider, IServiceDiscoveryEventsSender> implementationFactory)
    {
        builder.Services.AddSingleton(implementationFactory);
        return builder;
    }

    public static IServiceBeaconBuilder SetHerculesEventsSender(
        this IServiceBeaconBuilder builder,
        Action<HerculesServiceDiscoveryEventsSenderOptions, IServiceProvider> configure)
    {
        builder.Services.AddSingleton(HerculesServiceDiscoveryEventsSenderFactory.Create);
        return builder;
    }
}