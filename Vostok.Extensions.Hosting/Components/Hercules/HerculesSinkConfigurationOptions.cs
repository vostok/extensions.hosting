using JetBrains.Annotations;

using Vostok.Clusterclient.Core.Topology;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.Hercules.Client;

namespace Vostok.Extensions.Hosting.Components.Hercules;

[PublicAPI]
public class HerculesSinkConfigurationOptions
{
    public Func<string?> ApiKeyProvider { get; set; } = () => null;
    public bool VerboseLogging { get; set; }
    internal ClusterProviderBuilder? ClusterProviderBuilder { get; private set; }

    private readonly List<Action<HerculesSinkSettings>> _settingsCustomizations = [];

    public void SetClusterConfigTopology(string path) =>
        ClusterProviderBuilder = ClusterProviderBuilder.FromClusterConfig(path);

    public void SetExternalUrlTopology(string url) =>
        SetClusterProvider(new FixedClusterProvider(url));

    public void SetClusterProvider(IClusterProvider clusterProvider) =>
        ClusterProviderBuilder = ClusterProviderBuilder.FromValue(clusterProvider);

    public void SetServiceDiscoveryTopology(string environment, string application) =>
        ClusterProviderBuilder = ClusterProviderBuilder.FromServiceDiscovery(environment, application);

    public void CustomizeSettings(Action<HerculesSinkSettings> customization) =>
        _settingsCustomizations.Add(customization);

    internal void ApplyCustomizations(HerculesSinkSettings settings) =>
        _settingsCustomizations.ForEach(customization => customization(settings));
}