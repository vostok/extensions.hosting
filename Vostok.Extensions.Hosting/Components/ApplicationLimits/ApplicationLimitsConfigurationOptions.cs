using JetBrains.Annotations;

namespace Vostok.Extensions.Hosting.Components.ApplicationLimits;

[PublicAPI]
public sealed class ApplicationLimitsConfigurationOptions
{
    public float? CpuUnits { get; set; }
    public long? MemoryBytes { get; set; }
}