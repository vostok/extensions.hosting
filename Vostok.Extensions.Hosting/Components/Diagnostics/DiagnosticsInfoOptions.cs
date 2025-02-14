namespace Vostok.Extensions.Hosting.Components.Diagnostics;

public sealed class DiagnosticsInfoOptions
{
    public bool AddEnvironmentInfo { get; set; } = true;

    public bool AddLoadedAssembliesInfo { get; set; } = true;

    public bool AddSystemMetricsInfo { get; set; } = true;

    public bool AddApplicationMetricsInfo { get; set; } = true;

    public bool AddApplicationInfo { get; set; } = true;

    public bool AddHealthChecksInfo { get; set; } = true;

    public bool AddConfigurationInfo { get; set; }

    public bool AddHerculesSinkInfo { get; set; } = true;
}