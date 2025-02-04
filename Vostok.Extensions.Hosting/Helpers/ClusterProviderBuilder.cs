using Microsoft.Extensions.DependencyInjection;

using Vostok.Clusterclient.Core.Topology;
using Vostok.Clusterclient.Topology.CC;
using Vostok.Clusterclient.Topology.SD;
using Vostok.ClusterConfig.Client.Abstractions;
using Vostok.Extensions.Hosting.Components.ClusterConfigClient;
using Vostok.Extensions.Hosting.Components.ServiceDiscovery;
using Vostok.Logging.Abstractions;
using Vostok.ServiceDiscovery.Abstractions;

namespace Vostok.Extensions.Hosting.Helpers;

internal sealed class ClusterProviderBuilder
{
    private readonly Func<IServiceProvider, IClusterProvider?> _providerFactory;

    private ClusterProviderBuilder(Func<IServiceProvider, IClusterProvider?> providerFactory)
    {
        _providerFactory = providerFactory;
    }

    public static ClusterProviderBuilder FromValue(IClusterProvider value) => new(_ => value);

    public static ClusterProviderBuilder FromClusterConfig(string path)
    {
        return new ClusterProviderBuilder(GetProviderFactory);

        IClusterProvider? GetProviderFactory(IServiceProvider provider)
        {
            var clusterConfigClient = provider.GetRequiredService<IClusterConfigClient>();
            if (clusterConfigClient is DisabledClusterConfigClient) return null;

            var log = provider.GetRequiredService<ILog>();

            return new ClusterConfigClusterProvider(clusterConfigClient, path, log);
        }
    }
    
    public static ClusterProviderBuilder FromServiceDiscovery(string environment, string application)
    {
        return new ClusterProviderBuilder(GetProviderFactory);

        IClusterProvider? GetProviderFactory(IServiceProvider provider)
        {
            var serviceLocator = provider.GetRequiredService<IServiceLocator>();
            if (serviceLocator is DevNullServiceLocator)
                return null;

            var log = provider.GetRequiredService<ILog>();

            return new ServiceDiscoveryClusterProvider(serviceLocator, environment, application, log);
        }
    }

    public IClusterProvider? Build(IServiceProvider provider) => _providerFactory(provider);
}