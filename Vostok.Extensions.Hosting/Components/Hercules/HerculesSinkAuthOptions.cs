namespace Vostok.Extensions.Hosting.Components.Hercules;

internal sealed class HerculesSinkAuthOptions
{
    public string? ApiKey { get; set; }

    public bool NeedConfigureCustomKeys() => string.IsNullOrEmpty(ApiKey);
}