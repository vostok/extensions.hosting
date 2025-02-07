namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

public sealed class HerculesServiceDiscoveryEventsSenderOptions
{
    public string? StreamName { get; set; }
    public Func<string?> ApiKeyProvider { get; set; } = () => null;
}