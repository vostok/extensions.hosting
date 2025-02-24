using System.Diagnostics.Metrics;

using Microsoft.Extensions.Hosting;

using Vostok.Extensions.Hosting.OpenTelemetry;
using Vostok.Metrics;

namespace Vostok.Extensions.Hosting.Components.Logging.LevelCounter;

internal sealed class LogLevelMetricsHostedService(
    ILogLevelCountersCollector logLevelCountersCollector,
    VostokMeters vostokMeters) : IHostedService
{
    private static readonly KeyValuePair<string, object?> ComponentTag = new(WellKnownTagKeys.Component, "VostokLog");

    public Task StartAsync(CancellationToken cancellationToken)
    {
        vostokMeters.Instance.CreateObservableCounter(
            "EventsByLevel",
            ProvideMeasurements,
            null,
            "Number of events with each Vostok log level.");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private IEnumerable<Measurement<int>> ProvideMeasurements()
    {
        var counters = logLevelCountersCollector.Collect();

        yield return new Measurement<int>(counters.Debug, ComponentTag, LevelTag("Debug"));
        yield return new Measurement<int>(counters.Info, ComponentTag, LevelTag("Info"));
        yield return new Measurement<int>(counters.Warn, ComponentTag, LevelTag("Warn"));
        yield return new Measurement<int>(counters.Error, ComponentTag, LevelTag("Error"));
        yield return new Measurement<int>(counters.Fatal, ComponentTag, LevelTag("Fatal"));

        yield break;

        static KeyValuePair<string, object?> LevelTag(string logLevel) => new("LogLevel", logLevel);
    }
}