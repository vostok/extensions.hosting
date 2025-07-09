using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Components.Logging.File;
using Vostok.Extensions.Hosting.Components.Logging.Internal;
using Vostok.Extensions.Hosting.Components.ServiceDiscovery.ZooKeeperClient;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.Hosting.Abstractions;
using Vostok.Logging.Abstractions;
using Vostok.Logging.File.Configuration;
using Vostok.ServiceDiscovery;
using Vostok.ServiceDiscovery.Abstractions;
using Vostok.ServiceDiscovery.Telemetry;
using Vostok.ZooKeeper.Client.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery.ServiceBeacon;

internal static class ServiceBeaconFactory
{
    public static IServiceBeacon Create(IServiceProvider provider)
    {
        var settings = provider.GetRequiredService<IOptions<ServiceBeaconSettings>>().Value;
        var log = provider.GetInitializationLog();
        var zooKeeperClient = provider.GetService<IZooKeeperClient>();

        if (zooKeeperClient is null or DevNullZooKeeperClient)
        {
            log.LogDisabled("ServiceLocator", "disabled ZooKeeperClient");
            return new DevNullServiceBeacon(CreateDevNullReplicaInfo(settings, log, provider));
        }

        return CreateBeacon(settings, zooKeeperClient, log, provider);
    }

    public static IServiceBeacon CreateDisabled(IServiceProvider provider)
    {
        var settings = provider.GetRequiredService<IOptions<ServiceBeaconSettings>>().Value;
        var log = provider.GetInitializationLog();
        return new DevNullServiceBeacon(CreateDevNullReplicaInfo(settings, log, provider));
    }

    private static Vostok.ServiceDiscovery.ServiceBeacon CreateBeacon(
        ServiceBeaconSettings settings,
        IZooKeeperClient zooKeeperClient,
        ILog log,
        IServiceProvider provider)
    {
        if (provider.GetService<IServiceDiscoveryEventsContext>() is { } eventsContext)
            settings.ServiceDiscoveryEventContext = eventsContext;

        var applicationIdentity = provider.GetRequiredService<IVostokApplicationIdentity>();

        var logsDirectory = GetLogsDirectory(provider);
        var replicaCustomizations = provider.GetRequiredService<IOptions<ServiceBeaconMiscellaneousOptions>>().Value
                                            .ReplicaCustomizations;

        return new Vostok.ServiceDiscovery.ServiceBeacon(
            zooKeeperClient,
            replica =>
            {
                replica.SetEnvironment(applicationIdentity.Environment);
                replica.SetApplication(applicationIdentity.FormatServiceName());

                replica.SetProperty(WellKnownApplicationIdentityProperties.Project, applicationIdentity.Project);
                replica.SetProperty(WellKnownApplicationIdentityProperties.Subproject, applicationIdentity.Subproject);
                replica.SetProperty(WellKnownApplicationIdentityProperties.Environment, applicationIdentity.Environment);
                replica.SetProperty(WellKnownApplicationIdentityProperties.Application, applicationIdentity.Application);
                replica.SetProperty(WellKnownApplicationIdentityProperties.Instance, applicationIdentity.Instance);

                replica.SetProperty("logsDirectory", logsDirectory);

                foreach (var customization in replicaCustomizations)
                    customization(replica);
            },
            settings,
            log);
    }

    private static IReplicaInfo CreateDevNullReplicaInfo(
        ServiceBeaconSettings settings,
        ILog log,
        IServiceProvider provider)
        => CreateBeacon(settings, new DevNullZooKeeperClient(), log, provider).ReplicaInfo;

    private static string? GetLogsDirectory(IServiceProvider provider)
    {
        try
        {
            if (provider.GetKeyedService<LogRegistration>(LogRegistration.Key.For(FileLogFactory.LogName)) is null)
                return null; // File log disabled

            var fileLogSettings = provider.GetRequiredService<IOptions<FileLogSettings>>().Value;

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileLogSettings.FilePath);
            return Path.GetDirectoryName(path);
        }
        catch (Exception)
        {
            return null;
        }
    }
}