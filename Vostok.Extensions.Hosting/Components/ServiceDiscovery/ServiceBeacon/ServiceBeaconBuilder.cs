using Microsoft.Extensions.DependencyInjection;

using Vostok.ServiceDiscovery;
using Vostok.ServiceDiscovery.Telemetry.EventsSender;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery.ServiceBeacon;

internal sealed class ServiceBeaconBuilder(IServiceCollection services) : IServiceBeaconBuilder
{
    public IServiceCollection Services { get; } = services;

    public IServiceBeaconBuilder Configure(Action<ServiceBeaconSettings, IServiceProvider> configure)
    {
        Services.AddOptions<ServiceBeaconSettings>().Configure(configure);
        return this;
    }

    public IServiceBeaconBuilder ConfigureRegistration(
        bool waitRegistration,
        TimeSpan? registrationWaitTimeout = null)
    {
        Services.Configure<ServiceBeaconMiscellaneousOptions>(options =>
        {
            options.WaitRegistration = waitRegistration;
            if (registrationWaitTimeout.HasValue)
                options.RegistrationWaitTimeout = registrationWaitTimeout.Value;
        });

        return this;
    }

    public IServiceBeaconBuilder CustomizeReplicaInfo(Func<IServiceProvider, ReplicaInfoSetup> implementationFactory)
    {
        Services.AddOptions<ServiceBeaconMiscellaneousOptions>()
                .Configure<IServiceProvider>((options, provider) =>
                {
                    options.ReplicaCustomizations.Add(implementationFactory(provider));
                });

        return this;
    }

    public IServiceBeaconBuilder SetEventSender(Func<IServiceProvider, IServiceDiscoveryEventsSender> implementationFactory)
    {
        // Note: Don't use TryAdd or TryAddEnumerable because we have no 'composite' sender and always use last registered. 
        Services.AddSingleton(implementationFactory);
        return this;
    }
}