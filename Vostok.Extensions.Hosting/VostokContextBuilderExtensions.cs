using Microsoft.Extensions.DependencyInjection;

using Vostok.ClusterConfig.Client;
using Vostok.Extensions.Hosting.Components.ApplicationIdentity;
using Vostok.Extensions.Hosting.Components.ApplicationReplicationInfo;
using Vostok.Extensions.Hosting.Components.ClusterConfigClient;
using Vostok.Extensions.Hosting.Components.Datacenters;

namespace Vostok.Extensions.Hosting;

public static class VostokContextBuilderExtensions
{
    public static IVostokContextBuilder ConfigureApplicationIdentity(
        this IVostokContextBuilder builder,
        Action<ApplicationIdentityOptions> configure)
    {
        builder.Services.Configure(configure);

        return builder;
    }
    
    public static IVostokContextBuilder ConfigureApplicationReplicationInfo(
        this IVostokContextBuilder builder,
        Action<ApplicationReplicationInfoConfigurationOptions> configure)
    {
        builder.Services.Configure(configure);

        return builder;
    }

    public static IVostokContextBuilder ConfigureClusterConfigClient(
        this IVostokContextBuilder builder,
        Action<ClusterConfigClientSettings> configure)
    {
        builder.Services.Configure<ClusterConfigClientConfigurationOptions>(
            options => options.AddCustomization(configure));

        return builder;
    }

    public static IVostokContextBuilder ConfigureDatacenters(
        this IVostokContextBuilder builder,
        Action<DatacentersConfigurationOptions> configure)
    {
        builder.Services.Configure(configure);

        return builder;
    }
}