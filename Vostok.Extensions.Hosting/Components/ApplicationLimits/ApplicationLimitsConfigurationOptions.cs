namespace Vostok.Extensions.Hosting.Components.ApplicationLimits;

public sealed class ApplicationLimitsConfigurationOptions
{
    internal Func<float?> CpuUnitsProvider { get; private set; } = () => null;
    internal Func<long?> MemoryBytesProvider { get; private set; } = () => null;

    public void SetCpuUnits(float? cpuUnits) =>
        CpuUnitsProvider = () => cpuUnits;

    public void SetCpuUnitsProvider(Func<float?> cpuUnitsProvider) =>
        CpuUnitsProvider = cpuUnitsProvider ?? throw new ArgumentNullException(nameof(cpuUnitsProvider));

    public void SetMemoryBytes(long? memoryBytes) =>
        MemoryBytesProvider = () => memoryBytes;

    public void SetMemoryBytesProvider(Func<long?> memoryBytesProvider) =>
        MemoryBytesProvider = memoryBytesProvider ?? throw new ArgumentNullException(nameof(memoryBytesProvider));
}