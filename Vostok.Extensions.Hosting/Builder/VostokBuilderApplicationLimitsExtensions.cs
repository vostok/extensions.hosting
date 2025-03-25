using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.Components.ApplicationLimits;
using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class VostokBuilderApplicationLimitsExtensions
{
    public static IVostokBuilder WithApplicationLimits(
        this IVostokBuilder builder,
        Action<ApplicationLimitsOptions> configure)
    {
        builder.WithApplicationLimits((options, _) => configure(options));

        return builder;
    }

    public static IVostokBuilder WithApplicationLimits(
        this IVostokBuilder builder,
        Action<ApplicationLimitsOptions, IServiceProvider>? configure = null)
    {
        builder.Services.AddApplicationLimits();

        if (configure is not null)
            builder.Services.AddOptions<ApplicationLimitsOptions>().Configure(configure);

        return builder;
    }

    private static void AddApplicationLimits(this IServiceCollection services) =>
        services.TryAddSingleton<IVostokApplicationLimits, ApplicationLimits>();
}