using JetBrains.Annotations;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery.EventsContext;

[PublicAPI]
public sealed class HerculesServiceDiscoveryEventsSenderOptions
{
    public string? Stream { get; set; }
}