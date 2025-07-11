using Microsoft.Extensions.DependencyInjection;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.ServiceDiscovery.Telemetry;
using Vostok.ServiceDiscovery.Telemetry.EventsSender;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery.ServiceBeacon.EventsContext;

internal static class ServiceDiscoveryEventsContextFactory
{
    public static IServiceDiscoveryEventsContext Create(IServiceProvider provider)
    {
        var eventsSender = provider.GetService<IServiceDiscoveryEventsSender>();
        if (eventsSender is null)
        {
            provider.GetInitializationLog().LogDisabled("ServiceDiscoveryEventsContext", "Unconfigured ServiceDiscoveryEventsSender");
            return new DevNullServiceDiscoveryEventsContext();
        }

        var serviceDiscoveryEventsContextConfig = new ServiceDiscoveryEventsContextConfig(eventsSender);
        return new ServiceDiscoveryEventsContext(serviceDiscoveryEventsContextConfig);
    }
}