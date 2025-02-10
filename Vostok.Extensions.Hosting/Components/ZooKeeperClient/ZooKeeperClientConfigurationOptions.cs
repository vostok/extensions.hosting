using JetBrains.Annotations;

using Vostok.Clusterclient.Core.Topology;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.ZooKeeper.Client;
using Vostok.ZooKeeper.Client.Abstractions;
using Vostok.ZooKeeper.Client.Abstractions.Model.Authentication;

namespace Vostok.Extensions.Hosting.Components.ZooKeeperClient;

[PublicAPI]
public sealed class ZooKeeperClientConfigurationOptions
{
    internal string? ConnectionString { get; private set; }
    internal ClusterProviderBuilder? ClusterProviderBuilder { get; private set; }

    private readonly List<Action<ZooKeeperClientSettings>> _settingsCustomizations = [];
    private readonly List<AuthenticationInfo> _authenticationInfos = [];

    public void SetClusterProvider(IClusterProvider clusterProvider)
    {
        ConnectionString = null;
        ClusterProviderBuilder = ClusterProviderBuilder.FromValue(clusterProvider);
    }

    public void SetConnectionString(string connectionString)
    {
        ClusterProviderBuilder = null;
        ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public void SetClusterConfigTopology(string path)
    {
        ConnectionString = null;
        ClusterProviderBuilder = ClusterProviderBuilder.FromClusterConfig(path);
    }

    public void CustomizeSettings(Action<ZooKeeperClientSettings> customization) =>
        _settingsCustomizations.Add(customization);

    public void AddAuthenticationInfo(AuthenticationInfo authenticationInfo) =>
        _authenticationInfos.Add(authenticationInfo);

    internal void ApplyCustomizations(ZooKeeperClientSettings settings) =>
        _settingsCustomizations.ForEach(customization => customization(settings));

    internal void ApplyAuthenticationInfos(IZooKeeperClient client) =>
        _authenticationInfos.ForEach(client.AddAuthenticationInfo);
}