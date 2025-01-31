using Microsoft.Extensions.DependencyInjection;

using Vostok.ClusterConfig.Client;
using Vostok.Extensions.Hosting.Components.ApplicationIdentity;
using Vostok.Extensions.Hosting.Components.ApplicationLimits;
using Vostok.Extensions.Hosting.Components.ApplicationReplicationInfo;
using Vostok.Extensions.Hosting.Components.ClusterConfigClient;
using Vostok.Extensions.Hosting.Components.Datacenters;

namespace Vostok.Extensions.Hosting;

public static class VostokContextBuilderExtensions
{
    public static IVostokBuilder ConfigureApplicationIdentity(
        this IVostokBuilder builder,
        Action<ApplicationIdentityOptions> configure)
    {
        builder.Services.Configure(configure);

        return builder;
    }
    
    public static IVostokBuilder ConfigureApplicationReplicationInfo(
        this IVostokBuilder builder,
        Action<ApplicationReplicationInfoConfigurationOptions> configure)
    {
        builder.Services.Configure(configure);

        return builder;
    }

    public static IVostokBuilder ConfigureApplicationLimits(
        this IVostokBuilder builder,
        Action<ApplicationLimitsConfigurationOptions> configure)
    {
        builder.Services.Configure(configure);

        return builder;
    }

    public static IVostokBuilder ConfigureClusterConfigClient(
        this IVostokBuilder builder,
        Action<ClusterConfigClientSettings> configure)
    {
        builder.Services.Configure<ClusterConfigClientConfigurationOptions>(
            options => options.AddCustomization(configure));

        return builder;
    }

    public static IVostokBuilder ConfigureDatacenters(
        this IVostokBuilder builder,
        Action<DatacentersConfigurationOptions> configure)
    {
        builder.Services.Configure(configure);

        return builder;
    }
}