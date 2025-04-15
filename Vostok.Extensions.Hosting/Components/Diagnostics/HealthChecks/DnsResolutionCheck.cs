using Vostok.Hosting.Abstractions.Diagnostics;
using Vostok.Metrics.System.Process;

namespace Vostok.Extensions.Hosting.Components.Diagnostics.HealthChecks;

internal sealed class DnsResolutionCheck : IHealthCheck, IObserver<CurrentProcessMetrics>, IDisposable
{
    private const double FailedLookupsThreshold = 0.2;

    private static readonly TimeSpan Period = TimeSpan.FromMinutes(5);

    private readonly IDisposable _subscription;

    private DnsStatistics _dnsStatistics;

    public DnsResolutionCheck(CurrentProcessMonitor monitor) =>
        _subscription = monitor.ObserveMetrics(Period).Subscribe(this);

    public Task<HealthCheckResult> CheckAsync(CancellationToken cancellationToken)
    {
        if (_dnsStatistics.FailedLookupsFraction > FailedLookupsThreshold)
        {
            var healthCheckResult = HealthCheckResult.Degraded(
                $"Percentage of failed DNS lookups exceeded: {_dnsStatistics.FailedLookupsFraction * 100} %");
            
            return Task.FromResult(healthCheckResult);
        }

        return Task.FromResult(HealthCheckResult.Healthy());
    }

    public void OnNext(CurrentProcessMetrics value)
    {
        _dnsStatistics = new DnsStatistics(value.FailedDnsLookupsCount ?? 0, value.DnsLookupsCount ?? 0);
    }

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

    private struct DnsStatistics(int failedLookupsCount, int lookupsCount)
    {
        public double FailedLookupsFraction => lookupsCount != 0 ? (double)failedLookupsCount / lookupsCount : 0;
    }
}