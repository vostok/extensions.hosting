using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.Components.ServiceDiscovery;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class ServiceBeaconBuilderExtensions
{
    public static IVostokBuilder WithServiceBeacon(
        this IVostokBuilder builder,
        Action<ServiceBeaconConfigurationOptions> configure)
    {
        builder.WithServiceBeacon((settings, _) => configure(settings));

        return builder;
    }

    public static IVostokBuilder WithServiceBeacon(
        this IVostokBuilder builder,
        Action<ServiceBeaconConfigurationOptions, IServiceProvider>? configure = null)
    {
        builder.Services.AddServiceBeacon();

        if (configure is not null)
            builder.Services.AddOptions<ServiceBeaconConfigurationOptions>().Configure(configure);

        return builder;
    }

    private static void AddServiceBeacon(this IServiceCollection services)
    {
        services.TryAddSingleton(ServiceBeaconFactory.Create);
        services.TryAddSingleton(ServiceDiscoveryEventsContextFactory.Create);
        services.AddHostedService<ServiceBeaconHostedService>();
    }
}