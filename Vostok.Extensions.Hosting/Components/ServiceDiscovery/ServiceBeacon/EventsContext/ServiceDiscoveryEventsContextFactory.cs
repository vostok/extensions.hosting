using Microsoft.Extensions.DependencyInjection;

using Vostok.ServiceDiscovery.Telemetry;
using Vostok.ServiceDiscovery.Telemetry.EventsSender;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery.ServiceBeacon.EventsContext;

internal static class ServiceDiscoveryEventsContextFactory
{
    public static IServiceDiscoveryEventsContext Create(IServiceProvider services)
    {
        var eventsSender = services.GetService<IServiceDiscoveryEventsSender>();
        if (eventsSender is null)
            return new DevNullServiceDiscoveryEventsContext();

        var serviceDiscoveryEventsContextConfig = new ServiceDiscoveryEventsContextConfig(eventsSender);
        return new ServiceDiscoveryEventsContext(serviceDiscoveryEventsContextConfig);
    }
}