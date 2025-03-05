using Microsoft.Extensions.Options;

using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ApplicationLimits;

internal class ApplicationLimits(IOptionsMonitor<ApplicationLimitsConfigurationOptions> options)
    : IVostokApplicationLimits
{
    public float? CpuUnits => options.CurrentValue.CpuUnits;
    public long? MemoryBytes => options.CurrentValue.MemoryBytes;
}