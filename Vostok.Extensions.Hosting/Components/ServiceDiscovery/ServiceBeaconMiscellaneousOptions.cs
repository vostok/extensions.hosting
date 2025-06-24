using Vostok.ServiceDiscovery;
using Vostok.ServiceDiscovery.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

internal sealed class ServiceBeaconMiscellaneousOptions
{
    /// <summary>
    /// If enabled, host will wait for <see cref="IServiceBeacon"/> start.
    /// </summary>
    public bool WaitRegistration { get; set; }

    /// <summary>
    /// If enabled, host will wait for <see cref="IServiceBeacon"/> start even if the application does not require a port.
    /// </summary>
    public TimeSpan RegistrationWaitTimeout { get; set; } = TimeSpan.FromSeconds(10);

    public List<ReplicaInfoSetup> ReplicaCustomizations { get; set; } = [];
}