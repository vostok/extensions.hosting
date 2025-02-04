using Microsoft.Extensions.Options;

using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ApplicationLimits;

internal class ApplicationLimits(IOptions<ApplicationLimitsConfigurationOptions> options) : IVostokApplicationLimits
{
    private readonly Func<float?> _cpuUnitsProvider = options.Value.CpuUnitsProvider;
    private readonly Func<long?> _memoryBytesProvider = options.Value.MemoryBytesProvider;

    public float? CpuUnits => _cpuUnitsProvider();
    public long? MemoryBytes => _memoryBytesProvider();
}