using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Datacenters;
using Vostok.Extensions.Hosting.Components.ZooKeeperClient;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.Hosting.Abstractions;
using Vostok.Logging.Abstractions;
using Vostok.ServiceDiscovery;
using Vostok.ServiceDiscovery.Abstractions;
using Vostok.ZooKeeper.Client.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

internal static class ServiceBeaconFactory
{
    public static IServiceBeacon Create(IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<ServiceBeaconConfigurationOptions>>().Value;
        var log = provider.GetRequiredService<ILog>();
        var zooKeeperClient = provider.GetService<IZooKeeperClient>();

        // todo: Enabled/Disabled

        if (zooKeeperClient is null or DevNullZooKeeperClient)
        {
            log.LogDisabled("ServiceLocator", "disabled ZooKeeperClient");
            return new DevNullServiceBeacon(CreateDevNullReplicaInfo(options, log, provider));
        }

        return CreateBeacon(options, zooKeeperClient, log, provider);
    }

    private static ServiceBeacon CreateBeacon(
        ServiceBeaconConfigurationOptions options,
        IZooKeeperClient zooKeeperClient,
        ILog log,
        IServiceProvider provider)
    {
        var settings = new ServiceBeaconSettings
        {
            // TODO: Add after ServiceDiscoveryEventsContext registration
            //ServiceDiscoveryEventContext = context.ServiceDiscoveryEventsContext
        };

        if (options.RegistrationDeniedFromNonActiveDatacenters)
        {
            var datacenters = provider.GetRequiredService<IDatacenters>();
            settings.RegistrationAllowedProvider = datacenters.LocalDatacenterIsActive;
        }

        options.ApplyCustomizations(settings);

        var applicationIdentity = provider.GetRequiredService<IVostokApplicationIdentity>();

        return new ServiceBeacon(
            zooKeeperClient,
            s =>
            {
                s.SetProperty(WellKnownApplicationIdentityProperties.Project, applicationIdentity.Project);
                s.SetProperty(WellKnownApplicationIdentityProperties.Subproject, applicationIdentity.Subproject);
                s.SetProperty(WellKnownApplicationIdentityProperties.Environment, applicationIdentity.Environment);
                s.SetProperty(WellKnownApplicationIdentityProperties.Application, applicationIdentity.Application);
                s.SetProperty(WellKnownApplicationIdentityProperties.Instance, applicationIdentity.Instance);

                // TODO: Add after File log registration
                // s.SetProperty("logsDirectory", context.LogsDirectory);

                options.ApplyReplicaInfoCustomizations(s);
            },
            settings,
            log);
    }

    private static IReplicaInfo CreateDevNullReplicaInfo(
        ServiceBeaconConfigurationOptions options,
        ILog log,
        IServiceProvider provider)
        => CreateBeacon(options, new DevNullZooKeeperClient(), log, provider).ReplicaInfo;
}