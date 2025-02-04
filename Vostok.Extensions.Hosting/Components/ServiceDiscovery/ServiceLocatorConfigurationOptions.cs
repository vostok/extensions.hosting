using Vostok.ServiceDiscovery;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

internal sealed class ServiceLocatorConfigurationOptions
{
    private readonly List<Action<ServiceLocatorSettings>> _customizations = [];

    public void CustomizeSettings(Action<ServiceLocatorSettings> configure) =>
        _customizations.Add(configure);

    public void ApplyCustomizations(ServiceLocatorSettings settings) =>
        _customizations.ForEach(customization => customization(settings));
}