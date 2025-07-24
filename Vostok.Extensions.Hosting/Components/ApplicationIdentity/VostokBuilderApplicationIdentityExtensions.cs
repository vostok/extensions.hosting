using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ApplicationIdentity;

[PublicAPI]
public static class VostokBuilderApplicationIdentityExtensions
{
    public static IVostokBuilder WithApplicationIdentity(
        this IVostokBuilder vostok,
        Action<ApplicationIdentityOptions>? configure = null)
    {
        if (!vostok.CheckComponentAlreadyAdded("ApplicationIdentity"))
            vostok.Services.AddApplicationIdentity();

        if (configure is not null)
            vostok.Services.Configure(configure);

        return vostok;
    }

    private static void AddApplicationIdentity(this IServiceCollection services)
    {
        services.AddOptionsWithValidateOnStart<ApplicationIdentityOptions>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<ApplicationIdentityOptions>, ConfigureApplicationIdentityOptionsFromEnv>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<ApplicationIdentityOptions>, ApplicationIdentityOptionsValidator>());

        services.TryAddSingleton<IVostokApplicationIdentity, VostokApplicationIdentity>();
    }
}