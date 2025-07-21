using Vostok.Clusterclient.Core.Topology;
using Vostok.ZooKeeper.Client.Abstractions.Model.Authentication;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery.ZooKeeperClient;

internal sealed class ZooKeeperClientMiscellaneousOptions
{
    public string? ConnectionString { get; set; }
    public Func<IServiceProvider, IClusterProvider?>? ClusterProvider { get; set; }

    public List<AuthenticationInfo> AuthenticationInfos { get; } = [];
}