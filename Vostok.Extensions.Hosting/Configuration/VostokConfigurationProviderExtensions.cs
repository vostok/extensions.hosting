using JetBrains.Annotations;

using Microsoft.Extensions.Configuration;

using Vostok.ClusterConfig.Client;
using Vostok.Configuration.Microsoft;

namespace Vostok.Extensions.Hosting.Configuration;

[PublicAPI]
public static class ConfigurationBuilderVostokExtensions
{
    public static IConfigurationBuilder AddClusterConfig(this IConfigurationBuilder builder, string prefix)
    {
        var clusterConfigSource = new ClusterConfigSourceWithParsers(ClusterConfigClient.Default, prefix);
        return builder.AddVostok(clusterConfigSource);
    }
}