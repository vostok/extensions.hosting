using Microsoft.Extensions.DependencyInjection;

using Vostok.Extensions.Hosting.Components.ApplicationIdentity;
using Vostok.Extensions.Hosting.Components.ApplicationReplicationInfo;

namespace Vostok.Extensions.Hosting;

public static class VostokContextBuilderExtensions
{
    public static IVostokContextBuilder ConfigureApplicationIdentity(
        this IVostokContextBuilder builder,
        Action<ApplicationIdentityConfigurationOptions> configure)
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
}