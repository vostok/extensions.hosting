using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Diagnostics;
using Vostok.Metrics;
using Vostok.Metrics.Models;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

internal sealed class HealthCheckMetricsHostedService(IServiceProvider provider) : IHostedService, IObserver<HealthReport>
{
    private IMetricContext _context = null!;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var healthTracker = provider.GetService<IHealthTracker>();
        var metrics = provider.GetService<IVostokApplicationMetrics>();

        if (healthTracker is null || metrics is null)
            return Task.CompletedTask;

        _context = metrics.Instance.WithTag(WellKnownTagKeys.Component, "VostokHealthChecks");

        var subscription = healthTracker.ObserveReports().Subscribe(this);
        provider.RegisterDisposable(subscription);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public void OnNext(HealthReport value)
    {
        foreach (var check in value.Checks)
        {
            _context.Send(
                new MetricDataPoint(
                    Convert.ToDouble(check.Value.Status),
                    (WellKnownTagKeys.Name, "HealthCheckStatus"),
                    ("HealthCheckName", check.Key))
                {
                    Timestamp = value.Timestamp
                }
            );
        }
    }

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }
}