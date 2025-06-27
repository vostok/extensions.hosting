using JetBrains.Annotations;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery.ServiceBeacon.EventsContext;

[PublicAPI]
public sealed class HerculesServiceDiscoveryEventsSenderOptions
{
    public string? Stream { get; set; }
}