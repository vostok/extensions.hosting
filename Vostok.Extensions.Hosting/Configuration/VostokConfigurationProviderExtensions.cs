using JetBrains.Annotations;

using Microsoft.Extensions.Configuration;

using Vostok.ClusterConfig.Client;
using Vostok.ClusterConfig.Client.Abstractions;
using Vostok.Configuration.Microsoft;
using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Configuration;

[PublicAPI]
public static class ConfigurationBuilderVostokExtensions
{
    public static IConfigurationBuilder AddClusterConfig(this IConfigurationBuilder builder, string prefix)
    {
        var clusterConfigClient = builder.GetOrCreateInitialClient();
        var clusterConfigSource = new ClusterConfigSourceWithParsers(clusterConfigClient, prefix);
        return builder.AddVostok(clusterConfigSource);
    }

    private static IClusterConfigClient GetOrCreateInitialClient(this IConfigurationBuilder builder)
    {
        const string clientPropertyName = "ConfigurationClusterConfigClient";

        if (builder.Properties.TryGetValue(clientPropertyName, out var clientFromProperties))
            return (IClusterConfigClient)clientFromProperties;

        var settings = new ClusterConfigClientSettings { Log = new SilentLog() };
        var clusterConfigClient = new ClusterConfigClient(settings);
        var substitutableClusterConfigClient = new SubstitutableClusterConfigClient(clusterConfigClient);

        SubstitutableClusterConfigClient.Instance = substitutableClusterConfigClient;

        builder.Properties.Add(clientPropertyName, substitutableClusterConfigClient);

        return substitutableClusterConfigClient;
    }
}