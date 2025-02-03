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
        builder.ConfigureApplicationIdentity((options, _) => configure(options));

        return builder;
    }

    public static IVostokBuilder ConfigureApplicationIdentity(
        this IVostokBuilder builder,
        Action<ApplicationIdentityOptions, IServiceProvider> configure)
    {
        builder.Services.AddOptions<ApplicationIdentityOptions>().Configure(configure);

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
        builder.ConfigureClusterConfigClient((settings, _) => configure(settings));

        return builder;
    }

    public static IVostokBuilder ConfigureClusterConfigClient(
        this IVostokBuilder builder,
        Action<ClusterConfigClientSettings, IServiceProvider> configure)
    {
        builder.Services.AddOptions<ClusterConfigClientConfigurationOptions>()
               .Configure<IServiceProvider>((options, provider) =>
               {
                   options.AddCustomization(settings => configure(settings, provider));
               });

        return builder;
    }

    public static IVostokBuilder ConfigureDatacenters(
        this IVostokBuilder builder,
        Action<DatacentersConfigurationOptions> configure)
    {
        builder.ConfigureDatacenters((options, _) => configure(options));

        return builder;
    }

    public static IVostokBuilder ConfigureDatacenters(
        this IVostokBuilder builder,
        Action<DatacentersConfigurationOptions, IServiceProvider> configure)
    {
        builder.Services.AddOptions<DatacentersConfigurationOptions>().Configure(configure);

        return builder;
    }
}