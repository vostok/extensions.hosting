using Microsoft.Extensions.Diagnostics.HealthChecks;

using Vostok.Commons.Helpers.Observable;

namespace Vostok.Extensions.Hosting.Components.Diagnostics.HealthChecks;

internal sealed class HealthTracker : IHealthTracker, IHealthCheckPublisher
{
    private readonly CachingObservable<HealthReport> _reportsObservable = new();

    public HealthReport CurrentReport { get; set; } =
        new(new Dictionary<string, HealthReportEntry>(), HealthStatus.Healthy, TimeSpan.Zero);

    public DateTimeOffset CurrentReportTimestamp { get; set; } = DateTimeOffset.MinValue;

    public IObservable<HealthReport> ObserveReports() => _reportsObservable;

    Task IHealthCheckPublisher.PublishAsync(HealthReport report, CancellationToken _)
    {
        CurrentReportTimestamp = DateTimeOffset.UtcNow;
        CurrentReport = report;

        _reportsObservable.Next(report);

        return Task.CompletedTask;
    }
}