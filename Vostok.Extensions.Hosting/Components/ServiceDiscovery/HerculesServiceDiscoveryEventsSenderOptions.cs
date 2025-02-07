namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

public sealed class HerculesServiceDiscoveryEventsSenderOptions
{
    public bool Enabled { get; set; } = true;
    public string? Stream { get; set; }
    public Func<string?> ApiKeyProvider { get; set; } = () => null;
}