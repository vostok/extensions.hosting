using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;

using Vostok.Commons.Helpers.Disposable;
using Vostok.Commons.Helpers.Observable;
using Vostok.Commons.Time;
using Vostok.Hosting.Abstractions.Diagnostics;
using Vostok.Logging.Abstractions;

// ReSharper disable UseNullPropagation
// ReSharper disable MethodSupportsCancellation

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

internal sealed class HealthTracker : IHealthTracker, IDisposable
{
    private readonly TimeSpan _checkPeriod;
    private readonly ILog _log;

    private readonly CachingObservable<HealthStatus> _statusObservable;
    private readonly CachingObservable<HealthReport> _reportsObservable;
    private readonly CachingObservable<(HealthStatus previous, HealthStatus current)> _statusChangesObservable;
    private readonly ConcurrentDictionary<string, IHealthCheck> _checks;
    private readonly CancellationTokenSource _cancellation;

    private volatile HealthReport _currentReport;
    private volatile Task? _checkerTask;
    private volatile TaskCompletionSource<bool> _checkerSignal;

    public HealthTracker(TimeSpan checkPeriod, ILog log)
    {
        _checkPeriod = checkPeriod;
        _log = log.ForContext<HealthTracker>();

        var results = new Dictionary<string, HealthCheckResult>();
        _currentReport = new HealthReport(HealthStatus.Healthy, TimeSpan.Zero, DateTimeOffset.UtcNow, results);
        _statusObservable = new CachingObservable<HealthStatus>(_currentReport.Status);
        _reportsObservable = new CachingObservable<HealthReport>(_currentReport);
        _statusChangesObservable = new CachingObservable<(HealthStatus previous, HealthStatus current)>();
        _checks = new ConcurrentDictionary<string, IHealthCheck>(StringComparer.OrdinalIgnoreCase);
        _cancellation = new CancellationTokenSource();
        _checkerSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    public HealthStatus CurrentStatus => _currentReport.Status;

    public HealthReport CurrentReport => _currentReport;

    public IObservable<HealthStatus> ObserveStatus() => _statusObservable;

    public IObservable<HealthReport> ObserveReports() => _reportsObservable;

    public IObservable<(HealthStatus previous, HealthStatus current)> ObserveStatusChanges() => _statusChangesObservable;

    public IDisposable RegisterCheck(string name, IHealthCheck check)
    {
        if (!_checks.TryAdd(name, check))
            throw new InvalidOperationException($"Health check with name '{name}' is already registered.");

        ForceNextIteration();

        return new ActionDisposable(() => _checks.TryRemove(name, out _));
    }

    public IEnumerator<(string name, IHealthCheck check)> GetEnumerator() =>
        _checks.Select(pair => (pair.Key, pair.Value)).GetEnumerator();

    public void Dispose()
    {
        _cancellation.Cancel();

        if (_checkerTask != null)
            _checkerTask.ContinueWith(_ => { }).GetAwaiter().GetResult();
    }

    public void LaunchPeriodicalChecks(CancellationToken externalToken)
    {
        var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(externalToken, _cancellation.Token);

        Interlocked.Exchange(ref _checkerTask, Task.Run(() => RunPeriodicallyAsync(linkedCancellation.Token)));
    }

    public async Task<HealthReport> RunChecksAsync(CancellationToken cancellationToken)
    {
        var results = new Dictionary<string, HealthCheckResult>(StringComparer.OrdinalIgnoreCase);

        var watch = Stopwatch.StartNew();

        foreach (var pair in _checks)
        {
            cancellationToken.ThrowIfCancellationRequested();

            results[pair.Key] = await pair.Value.CheckSafeAsync(cancellationToken).ConfigureAwait(false);
        }

        var aggregateStatus = SelectStatus(results.Values);

        return new HealthReport(aggregateStatus, watch.Elapsed, DateTimeOffset.UtcNow, results);
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    private void ForceNextIteration()
        => Interlocked
            .Exchange(ref _checkerSignal,
                new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously))
            .TrySetResult(true);

    private async Task RunPeriodicallyAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var budget = TimeBudget.StartNew(_checkPeriod);

            var report = await RunChecksAsync(cancellationToken).ConfigureAwait(false);

            HandleReport(report);

            await Task.WhenAny(Task.Delay(budget.Remaining, cancellationToken), _checkerSignal.Task)
                .ConfigureAwait(false);
        }
    }

    private HealthStatus SelectStatus(IReadOnlyCollection<HealthCheckResult> results)
    {
        if (results.Any(r => r.Status == HealthStatus.Failing))
            return HealthStatus.Failing;

        if (results.Any(r => r.Status == HealthStatus.Degraded))
            return HealthStatus.Degraded;

        return HealthStatus.Healthy;
    }

    private void HandleReport(HealthReport report)
    {
        LogFailedHealthChecks(report);

        var oldReport = Interlocked.Exchange(ref _currentReport, report);

        _reportsObservable.Next(report);

        if (oldReport.Status != report.Status)
        {
            LogChangedHealthStatus(oldReport.Status, report.Status);
            _statusObservable.Next(report.Status);
            _statusChangesObservable.Next((oldReport.Status, report.Status));
        }
    }

    private void LogFailedHealthChecks(HealthReport report)
    {
        foreach (var pair in report.Checks)
        {
            if (pair.Value.Status == HealthStatus.Healthy)
                continue;

            _log.Warn(
                "Health check '{HealthCheckName}' yielded '{HealthStatus}' result. Reason: {HealthStatusReason}",
                pair.Key,
                pair.Value.Status,
                pair.Value.Reason);
        }
    }

    private void LogChangedHealthStatus(HealthStatus oldStatus, HealthStatus newStatus)
    {
        const string message = "Current health status changed from {OldHealthStatus} to {NewHealthStatus}.";

        if (newStatus == HealthStatus.Healthy)
            _log.Info(message, oldStatus, newStatus);
        else
            _log.Warn(message, oldStatus, newStatus);
    }
}