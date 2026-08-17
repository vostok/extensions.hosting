namespace Vostok.Extensions.Hosting.Components.Hercules;

internal sealed class HerculesSinkAuthOptions
{
    public string? ApiKey { get; set; }

    public string? CustomLoggingApiKey { get; set; }

    public string? CustomServiceDiscoveryEventsApiKey { get; set; }

    public string? CustomMetricsTaggedApiKey { get; set; }

    public string? CustomTracingApiKey { get; set; }

    public bool NeedConfigureCustomKeys() => string.IsNullOrEmpty(ApiKey);
}