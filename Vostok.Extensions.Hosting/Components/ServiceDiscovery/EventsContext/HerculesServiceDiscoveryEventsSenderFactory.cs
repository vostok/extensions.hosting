using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Hercules.Client.Abstractions;
using Vostok.ServiceDiscovery.Telemetry.EventsSender;
using Vostok.ServiceDiscovery.Telemetry.Hercules;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery.EventsContext;

public static class HerculesServiceDiscoveryEventsSenderFactory
{
    public static IServiceDiscoveryEventsSender Create(IServiceProvider services)
    {
        var sink = services.GetService<IHerculesSink>();
        var options = services.GetRequiredService<IOptions<HerculesServiceDiscoveryEventsSenderOptions>>().Value;
        if (sink is null)
            return new DevNullServiceDiscoveryEventsSender();

        var settings = new HerculesServiceDiscoveryEventsSenderSettings(sink, options.Stream);
        return new HerculesServiceDiscoveryEventsSender(settings);
    }
}