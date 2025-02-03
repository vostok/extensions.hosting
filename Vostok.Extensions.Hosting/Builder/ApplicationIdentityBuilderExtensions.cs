using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Components.ApplicationIdentity;
using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class ApplicationIdentityBuilderExtensions
{
    public static IVostokBuilder WithApplicationIdentity(
        this IVostokBuilder builder,
        Action<ApplicationIdentityOptions> configure)
    {
        builder.WithApplicationIdentity((options, _) => configure(options));

        return builder;
    }

    public static IVostokBuilder WithApplicationIdentity(
        this IVostokBuilder builder,
        Action<ApplicationIdentityOptions, IServiceProvider>? configure = null)
    {
        builder.Services.AddApplicationIdentity();

        if (configure is not null)
            builder.Services.AddOptions<ApplicationIdentityOptions>().Configure(configure);

        return builder;
    }

    private static void AddApplicationIdentity(this IServiceCollection services)
    {
        services.AddOptionsWithValidateOnStart<ApplicationIdentityOptions>();
        services.TryAddSingleton<IValidateOptions<ApplicationIdentityOptions>, ApplicationIdentityOptionsValidator>();

        services.TryAddSingleton<IVostokApplicationIdentity, ApplicationIdentity>();
    }
}