using Microsoft.Extensions.DependencyInjection;

using Vostok.Clusterclient.Core.Topology;
using Vostok.Clusterclient.Topology.CC;
using Vostok.Clusterclient.Topology.SD;
using Vostok.ClusterConfig.Client.Abstractions;
using Vostok.Extensions.Hosting.Components.ServiceDiscovery.ServiceLocator;
using Vostok.Logging.Abstractions;
using Vostok.ServiceDiscovery.Abstractions;

namespace Vostok.Extensions.Hosting.Helpers;

internal static class ClusterProviderFactory
{
    public static IClusterProvider? FromClusterConfig(IServiceProvider provider, string path)
    {
        var log = provider.GetInitializationLog();
        var clusterConfigClient = provider.GetService<IClusterConfigClient>();

        if (clusterConfigClient?.Get(path) is not null)
            return new ClusterConfigClusterProvider(clusterConfigClient, path, log);

        log.ForContext<VostokBuilder>().Warn("Could not get topology from ClusterConfig in path: {TopologyPath}.", path);
        return null;

    }

    public static IClusterProvider? FromServiceDiscovery(
        IServiceProvider provider,
        string environment,
        string application)
    {
        var log = provider.GetInitializationLog();
        var serviceLocator = provider.GetService<IServiceLocator>();

        if (serviceLocator is not null and not DevNullServiceLocator)
            return new ServiceDiscoveryClusterProvider(serviceLocator, environment, application, log);

        log.ForContext<VostokBuilder>().Warn("ServiceDiscovery disabled. Could not get topology for `{Service}` in `{Environment}`.", application, environment);
        return null;
    }
}