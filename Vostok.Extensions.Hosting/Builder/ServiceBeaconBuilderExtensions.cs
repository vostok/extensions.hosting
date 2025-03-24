using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.Components.ServiceDiscovery;
using Vostok.Extensions.Hosting.Components.ServiceDiscovery.EventsContext;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class ServiceBeaconBuilderExtensions
{
    public static IVostokBuilder WithServiceBeacon(this IVostokBuilder builder, Action<IServiceBeaconBuilder> configure)
    {
        builder.Services.AddServiceBeacon();

        var serviceBeaconBuilder = new ServiceBeaconBuilder(builder.Services);
        configure(serviceBeaconBuilder);

        return builder;
    }

    public static IVostokBuilder DisableServiceBeacon(this IVostokBuilder builder)
    {
        builder.Services.Replace(ServiceDescriptor.Singleton(ServiceBeaconFactory.CreateDisabled));
        return builder;
    }

    private static void AddServiceBeacon(this IServiceCollection services)
    {
        services.TryAddSingleton(ServiceBeaconFactory.Create);
        services.TryAddSingleton(ServiceDiscoveryEventsContextFactory.Create);
        services.AddHostedService<ServiceBeaconHostedService>();
    }
}