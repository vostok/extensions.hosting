using System.Diagnostics.Metrics;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Extensions.Hosting.OpenTelemetry;
using Vostok.Hosting.Abstractions.Diagnostics;
using Vostok.Metrics;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

internal sealed class HealthCheckMetricsHostedService(IServiceProvider provider, VostokMeters meters) : IHostedService, IObserver<HealthReport>
{
    private Gauge<int> _checkStatusGauge = null!;

    private static readonly KeyValuePair<string, object?> ComponentTag =
        new(WellKnownTagKeys.Component, "VostokHealthChecks");

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var healthTracker = provider.GetService<IHealthTracker>();

        if (healthTracker is null)
            return Task.CompletedTask;

        _checkStatusGauge = meters.Instance.CreateGauge<int>("HealthCheckStatus");

        var subscription = healthTracker.ObserveReports().Subscribe(this);
        provider.RegisterDisposable(subscription);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public void OnNext(HealthReport value)
    {
        foreach (var check in value.Checks)
        {
            var checkKeyTag = new KeyValuePair<string, object?>("HealthCheckName", check.Key);
            _checkStatusGauge.Record(Convert.ToInt32(check.Value.Status), ComponentTag, checkKeyTag);
        }
    }

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }
}