using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Logging.Abstractions;
using Vostok.ServiceDiscovery.Telemetry;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

internal static class ServiceDiscoveryEventsContextFactory
{
    public static IServiceDiscoveryEventsContext Create(IServiceProvider services)
    {
        var log = services.GetRequiredService<ILog>();
        var options = services.GetRequiredService<IOptions<ServiceBeaconConfigurationOptions>>().Value;

        var eventsSender = options.CreateSender(services);
        if (eventsSender is null)
        {
            log.LogDisabled("ServiceDiscoveryEventsContext", "not configured event sender");
            return new DevNullServiceDiscoveryEventsContext();
        }

        var serviceDiscoveryEventsContextConfig = new ServiceDiscoveryEventsContextConfig(eventsSender);
        return new ServiceDiscoveryEventsContext(serviceDiscoveryEventsContextConfig);
    }
}