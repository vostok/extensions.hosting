using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ApplicationLimits;

[PublicAPI]
public static class VostokBuilderApplicationLimitsExtensions
{
    public static IVostokBuilder WithApplicationLimits(
        this IVostokBuilder vostok,
        Action<ApplicationLimitsOptions>? configure = null)
    {
        if (!vostok.CheckComponentAlreadyAdded("ApplicationLimits"))
            vostok.Services.AddApplicationLimits();

        if (configure is not null)
            vostok.Services.Configure(configure);

        return vostok;
    }

    private static void AddApplicationLimits(this IServiceCollection services) =>
        services.TryAddSingleton<IVostokApplicationLimits, ApplicationLimits>();
}