using Vostok.ServiceDiscovery;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

public sealed class ServiceLocatorConfigurationOptions
{
    private readonly List<Action<ServiceLocatorSettings>> _customizations = [];

    public void CustomizeSettings(Action<ServiceLocatorSettings> configure) =>
        _customizations.Add(configure);

    internal void ApplyCustomizations(ServiceLocatorSettings settings) =>
        _customizations.ForEach(customization => customization(settings));
}