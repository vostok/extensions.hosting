using Microsoft.Extensions.DependencyInjection;

using Vostok.Clusterclient.Core.Topology;
using Vostok.Clusterclient.Topology.CC;
using Vostok.Clusterclient.Topology.SD;
using Vostok.ClusterConfig.Client.Abstractions;
using Vostok.Logging.Abstractions;
using Vostok.ServiceDiscovery.Abstractions;

namespace Vostok.Extensions.Hosting.Helpers;

internal static class ClusterProviderFactory
{
    public static IClusterProvider FromClusterConfig(IServiceProvider provider, string path)
    {
        var clusterConfigClient = provider.GetRequiredService<IClusterConfigClient>();
        var log = provider.GetRequiredService<ILog>();
        return new ClusterConfigClusterProvider(clusterConfigClient, path, log);
    }

    public static IClusterProvider FromServiceDiscovery(
        IServiceProvider provider,
        string environment,
        string application)
    {
        var serviceLocator = provider.GetRequiredService<IServiceLocator>();
        var log = provider.GetRequiredService<ILog>();
        return new ServiceDiscoveryClusterProvider(serviceLocator, environment, application, log);
    }
}