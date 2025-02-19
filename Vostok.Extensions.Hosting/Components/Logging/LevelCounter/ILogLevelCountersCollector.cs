namespace Vostok.Extensions.Hosting.Components.Logging.LevelCounter;

internal interface ILogLevelCountersCollector
{
    LogLevelCounters Collect();
}