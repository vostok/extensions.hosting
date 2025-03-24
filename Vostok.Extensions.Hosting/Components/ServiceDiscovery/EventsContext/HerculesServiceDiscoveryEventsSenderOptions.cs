using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery.EventsContext;

[PublicAPI]
public sealed class HerculesServiceDiscoveryEventsSenderOptions
{
    [DisallowNull]
    public string? Stream { get; set; }
}