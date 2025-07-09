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
        Action<HerculesSinkSettings> configure)
    {
        builder.Configure((settings, _) => configure(settings));
        return builder;
    }

    public static IHerculesSinkBuilder Configure(
        this IHerculesSinkBuilder builder,
        Action<HerculesSinkSettings, IServiceProvider>? configure = null)
    {
        if (configure is not null)
            builder.Services.AddOptions<HerculesSinkSettings>().Configure(configure);
        return builder;
    }

    public static IHerculesSinkBuilder SetApiKey(this IHerculesSinkBuilder builder, string? apiKey)
    {
        builder.Services.Configure<HerculesSinkAuthOptions>(o => o.ApiKey = apiKey);
        return builder;
    }

    public static IHerculesSinkBuilder SetClusterProvider(
        this IHerculesSinkBuilder builder,
        IClusterProvider clusterProvider)
    {
        builder.SetClusterProvider(_ => clusterProvider);
        return builder;
    }

    public static IHerculesSinkBuilder SetClusterProvider(
        this IHerculesSinkBuilder builder,
        Func<IServiceProvider, IClusterProvider> implementationFactory)
    {
        builder.Services.AddKeyedSingleton(
            HerculesSinkFactory.ClusterProviderKey,
            (provider, _) => implementationFactory(provider));

        return builder;
    }

    public static IHerculesSinkBuilder SetExternalUrlTopology(this IHerculesSinkBuilder builder, string url) =>
        builder.SetClusterProvider(new FixedClusterProvider(url));

    public static IHerculesSinkBuilder SetClusterConfigTopology(this IHerculesSinkBuilder builder, string path) =>
        builder.SetClusterProvider(provider => ClusterProviderFactory.FromClusterConfig(provider, path));

    public static IHerculesSinkBuilder SetServiceDiscoveryTopology(this IHerculesSinkBuilder builder, string environment, string application) =>
        builder.SetClusterProvider(provider => ClusterProviderFactory.FromServiceDiscovery(provider, environment, application));
}