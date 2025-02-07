using Vostok.ServiceDiscovery;
using Vostok.ServiceDiscovery.Telemetry.EventsSender;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

public sealed class ServiceBeaconConfigurationOptions
{
    public bool RegistrationDeniedFromNonActiveDatacenters { get; set; }

    private Func<IServiceProvider, IServiceDiscoveryEventsSender?> _eventSenderFactory = _ => null; // todo: Replace with config like in Metrics?
    private readonly List<Action<ServiceBeaconSettings>> _customizations = [];
    private readonly List<ReplicaInfoSetup> _replicaInfoCustomizations = [];

    public void CustomizeSettings(Action<ServiceBeaconSettings> configure) =>
        _customizations.Add(configure);

    public void CustomizeReplicaInfo(ReplicaInfoSetup configure) =>
        _replicaInfoCustomizations.Add(configure);

    public void SetEventsSender(Func<IServiceProvider, IServiceDiscoveryEventsSender?> factory) =>
        _eventSenderFactory = factory;

    internal void ApplyCustomizations(ServiceBeaconSettings settings) =>
        _customizations.ForEach(customization => customization(settings));

    internal void ApplyReplicaInfoCustomizations(IReplicaInfoBuilder replicaInfoBuilder) =>
        _replicaInfoCustomizations.ForEach(customization => customization(replicaInfoBuilder));

    internal IServiceDiscoveryEventsSender? CreateSender(IServiceProvider services) =>
        _eventSenderFactory(services);
}