using Vostok.ServiceDiscovery;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

public sealed class ServiceBeaconConfigurationOptions
{
    public bool RegistrationDeniedFromNonActiveDatacenters { get; set; }

    private readonly List<Action<ServiceBeaconSettings>> _customizations = [];
    private readonly List<ReplicaInfoSetup> _replicaInfoCustomizations = [];

    public void CustomizeSettings(Action<ServiceBeaconSettings> configure) =>
        _customizations.Add(configure);

    public void CustomizeReplicaInfo(ReplicaInfoSetup configure) =>
        _replicaInfoCustomizations.Add(configure);

    internal void ApplyCustomizations(ServiceBeaconSettings settings) =>
        _customizations.ForEach(customization => customization(settings));

    internal void ApplyReplicaInfoCustomizations(IReplicaInfoBuilder replicaInfoBuilder) =>
        _replicaInfoCustomizations.ForEach(customization => customization(replicaInfoBuilder));
}