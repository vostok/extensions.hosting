using System.Diagnostics.Metrics;

using Microsoft.Extensions.Diagnostics.HealthChecks;

using Vostok.Extensions.Hosting.Components.Diagnostics.Helpers;
using Vostok.Extensions.Hosting.OpenTelemetry;
using Vostok.Metrics;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

internal sealed class MetricsHealthCheckPublisher(VostokMeters meters) : IHealthCheckPublisher
{
    private readonly Gauge<int> _checkStatusGauge = meters.Instance.CreateGauge<int>("HealthCheckStatus");

    private static readonly KeyValuePair<string, object?> ComponentTag =
        new(WellKnownTagKeys.Component, "VostokHealthChecks");

    public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
    {
        foreach (var (name, entry) in report.Entries)
        {
            // note: Convert for metrics backward compatibility
            var vostokStatus = entry.Status.ToVostokStatus();

            var checkKeyTag = new KeyValuePair<string, object?>("HealthCheckName", name);

            _checkStatusGauge.Record(Convert.ToInt32(vostokStatus), ComponentTag, checkKeyTag);
        }

        return Task.CompletedTask;
    }
}