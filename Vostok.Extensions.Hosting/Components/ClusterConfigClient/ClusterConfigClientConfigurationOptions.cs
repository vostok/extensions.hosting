using Vostok.ClusterConfig.Client;

namespace Vostok.Extensions.Hosting.Components.ClusterConfigClient;

internal sealed class ClusterConfigClientConfigurationOptions
{
    private readonly List<Action<ClusterConfigClientSettings>> _customizations = [];

    public void CustomizeSettings(Action<ClusterConfigClientSettings> customization)
        => _customizations.Add(customization);

    public void ApplyCustomizations(ClusterConfigClientSettings settings) =>
        _customizations.ForEach(customization => customization(settings));
}