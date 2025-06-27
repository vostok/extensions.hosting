using Vostok.Configuration.Primitives;
using Vostok.Hosting.Abstractions.Diagnostics;
using Vostok.Metrics.System.Process;

namespace Vostok.Extensions.Hosting.Components.Diagnostics.DiagnosticInfo.Providers;

internal sealed class SystemMetricsProvider : IDiagnosticInfoProvider, IObserver<CurrentProcessMetrics>, IDisposable
{
    private static readonly TimeSpan ObservationPeriod = TimeSpan.FromSeconds(5);

    private readonly IDisposable _subscription;
    private volatile CurrentProcessMetrics _metrics = new();

    public SystemMetricsProvider(CurrentProcessMonitor monitor) =>
        _subscription = monitor.ObserveMetrics(ObservationPeriod).Subscribe(this);

    public object Query()
    {
        return new
        {
            CpuTotalCores = Environment.ProcessorCount,
            CpuUtilizedPercent = (_metrics.CpuUtilizedFraction * 100d).ToString("0.00"),
            CpuUtilizedCores = _metrics.CpuUtilizedCores.ToString("0.00"),
            MemoryResident = _metrics.MemoryResident.Bytes(),
            MemoryPrivate = _metrics.MemoryPrivate.Bytes(),
            GcHeapSize = _metrics.GcHeapSize.Bytes(),
            GcGen0Size = _metrics.GcGen0Size.Bytes(),
            GcGen1Size = _metrics.GcGen1Size.Bytes(),
            GcGen2Size = _metrics.GcGen2Size.Bytes(),
            GcLOHSize = _metrics.GcLOHSize.Bytes(),
            _metrics.GcGen0Collections,
            _metrics.GcGen1Collections,
            _metrics.GcGen2Collections,
            _metrics.GcTimePercent,
            ThreadPoolTotalThreads = _metrics.ThreadPoolTotalCount,
            ThreadPoolQueue = _metrics.ThreadPoolQueueLength,
            ThreadPoolWorkers = $"{_metrics.ThreadPoolBusyWorkers}/{_metrics.ThreadPoolMinWorkers}",
            ThreadPoolIO = $"{_metrics.ThreadPoolBusyIo}/{_metrics.ThreadPoolMinIo}",
            _metrics.HandlesCount,
            _metrics.ActiveTimersCount,
            AllocationRate = _metrics.GcAllocatedBytes.Bytes() / ObservationPeriod,
            ContentionsPerSecond = (_metrics.LockContentionCount / ObservationPeriod.TotalSeconds).ToString("0.00"),
            ExceptionsPerSecond = (_metrics.ExceptionsCount / ObservationPeriod.TotalSeconds).ToString("0.00")
        };
    }

    public void OnNext(CurrentProcessMetrics value)
        => _metrics = value;

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    public void Dispose()
    {
        _subscription.Dispose();
    }
}