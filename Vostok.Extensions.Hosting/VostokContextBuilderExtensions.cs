using Microsoft.Extensions.DependencyInjection;

using Vostok.Extensions.Hosting.Components.ApplicationIdentity;

namespace Vostok.Extensions.Hosting;

public static class VostokContextBuilderExtensions
{
    public static IVostokContextBuilder ConfigureApplicationIdentity(
        this IVostokContextBuilder builder,
        Action<VostokApplicationIdentityConfigurationOptions> configure)
    {
        builder.Services.Configure(configure);

        return builder;
    }
}