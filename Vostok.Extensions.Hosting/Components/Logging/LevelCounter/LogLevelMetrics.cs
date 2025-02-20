using System.Diagnostics.Metrics;

using Vostok.Extensions.Hosting.OpenTelemetry;
using Vostok.Metrics;

namespace Vostok.Extensions.Hosting.Components.Logging.LevelCounter;

internal sealed class LogLevelMetrics(ILogLevelCountersCollector logLevelCountersCollector, IMeterFactory meterFactory)
{
    private readonly Meter _meter =
        meterFactory.Create(
            TelemetryConstants.VostokLogMeterName,
            tags: [KeyValuePair.Create(WellKnownTagKeys.Component, (object?)TelemetryConstants.VostokLogMeterName)]);

    public void StartCollectingMetrics() =>
        _meter.CreateObservableCounter(
            "EventsByLevel",
            ProvideMeasurements,
            null,
            "Number of events with each Vostok log level.");

    private IEnumerable<Measurement<int>> ProvideMeasurements()
    {
        var counters = logLevelCountersCollector.Collect();

        yield return new Measurement<int>(counters.Debug, LevelTag("Debug"));
        yield return new Measurement<int>(counters.Info, LevelTag("Info"));
        yield return new Measurement<int>(counters.Warn, LevelTag("Warn"));
        yield return new Measurement<int>(counters.Error, LevelTag("Error"));
        yield return new Measurement<int>(counters.Fatal, LevelTag("Fatal"));

        yield break;

        static KeyValuePair<string, object?> LevelTag(string logLevel) => new("LogLevel", logLevel);
    }
}