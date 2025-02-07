using JetBrains.Annotations;

using Vostok.ClusterConfig.Client;

namespace Vostok.Extensions.Hosting.Components.ClusterConfigClient;

[PublicAPI]
public sealed class ClusterConfigClientConfigurationOptions
{
    private readonly List<Action<ClusterConfigClientSettings>> _customizations = [];

    public void CustomizeSettings(Action<ClusterConfigClientSettings> customization)
        => _customizations.Add(customization);

    internal void ApplyCustomizations(ClusterConfigClientSettings settings) =>
        _customizations.ForEach(customization => customization(settings));
}