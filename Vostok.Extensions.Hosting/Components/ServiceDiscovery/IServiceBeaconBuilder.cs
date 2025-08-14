using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Vostok.ServiceDiscovery;
using Vostok.ServiceDiscovery.Abstractions;
using Vostok.ServiceDiscovery.Telemetry.EventsSender;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

/// <summary>
/// Builder for configuring <see cref="IServiceBeacon"/>.
/// </summary>
[PublicAPI]
public interface IServiceBeaconBuilder
{
    /// <summary>
    /// The host <see cref="IServiceCollection"/> where <see cref="IServiceBeacon"/> is configured.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Configures <see cref="ServiceBeaconSettings"/>.
    /// </summary>
    /// <param name="configure">Delegate to configure <see cref="ServiceBeaconSettings"/>.</param>
    /// <returns><see cref="IServiceBeaconBuilder"/> for chain method calls.</returns>
    IServiceBeaconBuilder Configure(Action<ServiceBeaconSettings> configure);

    /// <summary>
    /// Configures <see cref="IServiceBeacon"/> policy.
    /// </summary>
    /// <param name="waitRegistration">If enabled, host will wait for <see cref="IServiceBeacon"/> start.</param>
    /// <param name="registrationWaitTimeout">Maximum timeout for <see cref="IServiceBeacon"/> start.</param>
    /// <returns><see cref="IServiceBeaconBuilder"/> for chain method calls.</returns>
    IServiceBeaconBuilder ConfigureRegistration(bool waitRegistration, TimeSpan? registrationWaitTimeout = null);

    /// <summary>
    /// Adds provided <see cref="ReplicaInfoSetup"/> for replica customization.
    /// </summary>
    /// <param name="implementationFactory">The <see cref="ReplicaInfoSetup"/> implementation factory.</param>
    /// <returns><see cref="IServiceBeaconBuilder"/> for chain method calls.</returns>
    IServiceBeaconBuilder CustomizeReplicaInfo(Func<IServiceProvider, ReplicaInfoSetup> implementationFactory);

    /// <summary>
    /// Sets provided <see cref="IServiceDiscoveryEventsSender"/>.
    /// </summary>
    /// <param name="implementationFactory">The <see cref="IServiceDiscoveryEventsSender"/> implementation factory.</param>
    /// <returns><see cref="IServiceBeaconBuilder"/> for chain method calls.</returns>
    IServiceBeaconBuilder SetEventSender(Func<IServiceProvider, IServiceDiscoveryEventsSender> implementationFactory);
}