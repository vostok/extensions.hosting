using JetBrains.Annotations;

using Microsoft.Extensions.Configuration;

using Vostok.ClusterConfig.Client.Abstractions;
using Vostok.Configuration.Microsoft;
using Vostok.Configuration.Sources.ClusterConfig;

namespace Vostok.Extensions.Hosting.Configuration;

[PublicAPI]
public static class ConfigurationBuilderVostokExtensions
{
    public static IConfigurationBuilder AddClusterConfigDeferred(
        this IConfigurationBuilder builder,
        string prefix,
        Action<ClusterConfigSourceSettings>? configure = null)
    {
        return builder.Add(new ClusterConfigDeferredConfigurationSource(prefix, configure));
    }

    public static IConfigurationBuilder AddClusterConfig(
        this IConfigurationBuilder builder,
        IClusterConfigClient clusterConfigClient,
        string prefix,
        Action<ClusterConfigSourceSettings>? configure = null)
    {
        var settings = new ClusterConfigSourceSettingsWithParsers(clusterConfigClient, prefix);
        configure?.Invoke(settings);

        var clusterConfigSource = new ClusterConfigSource(settings);
        return builder.AddVostok(clusterConfigSource);
    }
}