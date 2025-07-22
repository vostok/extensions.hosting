using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Vostok.ServiceDiscovery;
using Vostok.ServiceDiscovery.Telemetry.EventsSender;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

[PublicAPI]
public interface IServiceBeaconBuilder
{
    IServiceCollection Services { get; }

    IServiceBeaconBuilder Configure(Action<ServiceBeaconSettings, IServiceProvider> configure);
    IServiceBeaconBuilder ConfigureRegistration(bool waitRegistration, TimeSpan? registrationWaitTimeout = null);
    IServiceBeaconBuilder CustomizeReplicaInfo(Func<IServiceProvider, ReplicaInfoSetup> implementationFactory);
    IServiceBeaconBuilder SetEventSender(Func<IServiceProvider, IServiceDiscoveryEventsSender> implementationFactory);
}