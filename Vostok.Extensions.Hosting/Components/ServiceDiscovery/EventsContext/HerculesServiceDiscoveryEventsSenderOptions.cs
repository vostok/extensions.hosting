using JetBrains.Annotations;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery.EventsContext;

[PublicAPI]
public sealed class HerculesServiceDiscoveryEventsSenderOptions
{
    public required string Stream { get; set; }
}