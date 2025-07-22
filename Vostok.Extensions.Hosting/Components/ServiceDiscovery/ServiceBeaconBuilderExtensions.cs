using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Vostok.Commons.Helpers.Network;
using Vostok.Datacenters;
using Vostok.Hosting.Abstractions;
using Vostok.ServiceDiscovery;
using Vostok.ServiceDiscovery.Abstractions;
using Vostok.ServiceDiscovery.Telemetry.EventsSender;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

[PublicAPI]
public static class ServiceBeaconBuilderExtensions
{
    public static IServiceBeaconBuilder Configure(
        this IServiceBeaconBuilder builder,
        Action<ServiceBeaconSettings> configure) =>
        builder.Configure((settings, _) => configure(settings));

    public static IServiceBeaconBuilder CustomizeReplicaInfo(
        this IServiceBeaconBuilder builder,
        ReplicaInfoSetup configure) =>
        builder.CustomizeReplicaInfo(_ => configure);

    /// <summary>
    /// Applies given <paramref name="port"/> to <see cref="IServiceBeacon"/> configuration.
    /// </summary>
    public static IServiceBeaconBuilder SetPort(this IServiceBeaconBuilder builder, int port) =>
        builder.CustomizeReplicaInfo(replica => replica.SetPort(port));

    /// <summary>
    /// Applies random free port to <see cref="IServiceBeacon"/> configuration.
    /// </summary>
    public static IServiceBeaconBuilder SetRandomFreePort(this IServiceBeaconBuilder builder) =>
        builder.SetPort(FreeTcpPortFinder.GetFreePort());

    /// <summary>
    /// Applies given base url <paramref name="path"/> to <see cref="IServiceBeacon"/> configuration.
    /// </summary>
    public static IServiceBeaconBuilder SetBaseUrlPath(this IServiceBeaconBuilder builder, string path) =>
        builder.CustomizeReplicaInfo(replica => replica.SetUrlPath(path));

    /// <summary>
    /// Applies given url <paramref name="scheme"/> to <see cref="IServiceBeacon"/> configuration.
    /// </summary>
    public static IServiceBeaconBuilder SetUrlScheme(this IServiceBeaconBuilder builder, string scheme) =>
        builder.CustomizeReplicaInfo(replica => replica.SetScheme(scheme));

    /// <summary>
    /// Enables HTTPS scheme in <see cref="IServiceBeacon"/> configuration.
    /// </summary>
    public static IServiceBeaconBuilder SetHttpsScheme(this IServiceBeaconBuilder builder) =>
        builder.SetUrlScheme("https");

    /// <summary>
    /// Applies given <paramref name="environment"/> name to <see cref="IServiceBeacon"/> configuration. Note this environment might differ from app identity <see cref="IVostokApplicationIdentity.Environment"/>.
    /// </summary>
    public static IServiceBeaconBuilder SetEnvironment(this IServiceBeaconBuilder builder, string environment) =>
        builder.CustomizeReplicaInfo(replica => replica.SetEnvironment(environment));

    /// <summary>
    /// Applies given <paramref name="application"/> name to <see cref="IServiceBeacon"/> configuration. Note this application might differ from app identity <see cref="IVostokApplicationIdentity.Application"/>.
    /// </summary>
    public static IServiceBeaconBuilder SetApplication(this IServiceBeaconBuilder builder, string application) =>
        builder.CustomizeReplicaInfo(replica => replica.SetApplication(application));

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
        IServiceDiscoveryEventsSender eventsSender) =>
        builder.SetEventSender(_ => eventsSender);
}