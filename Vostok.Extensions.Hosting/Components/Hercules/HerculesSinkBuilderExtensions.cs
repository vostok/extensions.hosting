using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Vostok.Clusterclient.Core.Topology;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.Hercules.Client;

namespace Vostok.Extensions.Hosting.Components.Hercules;

[PublicAPI]
public static class HerculesSinkBuilderExtensions
{
    public static IHerculesSinkBuilder Configure(
        this IHerculesSinkBuilder builder,
        Action<HerculesSinkSettings> configure) =>
        builder.Configure((settings, _) => configure(settings));

    public static IHerculesSinkBuilder SetClusterProvider(
        this IHerculesSinkBuilder builder,
        IClusterProvider clusterProvider) =>
        builder.SetClusterProvider(_ => clusterProvider);

    public static IHerculesSinkBuilder SetExternalUrlTopology(this IHerculesSinkBuilder builder, string url) =>
        builder.SetClusterProvider(new FixedClusterProvider(url));

    public static IHerculesSinkBuilder SetClusterConfigTopology(this IHerculesSinkBuilder builder, string path) =>
        builder.SetClusterProvider(provider => ClusterProviderFactory.FromClusterConfig(provider, path));

    public static IHerculesSinkBuilder SetServiceDiscoveryTopology(
        this IHerculesSinkBuilder builder,
        string environment,
        string application) =>
        builder.SetClusterProvider(provider =>
            ClusterProviderFactory.FromServiceDiscovery(provider, environment, application));

    public static IHerculesSinkBuilder EnableMetrics(this IHerculesSinkBuilder builder)
    {
        builder.Services.AddHostedService<HerculesSinkMetricsHostedService>();
        return builder;
    }
}