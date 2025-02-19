using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Logging.LevelCounter;

internal sealed class LevelCountingLog : ILog, ILogLevelCountersCollector
{
    private readonly ConcurrentCounter _debugEvents = new();
    private readonly ConcurrentCounter _infoEvents = new();
    private readonly ConcurrentCounter _warnEvents = new();
    private readonly ConcurrentCounter _errorEvents = new();
    private readonly ConcurrentCounter _fatalEvents = new();

    public void Log(LogEvent? @event)
    {
        switch (@event?.Level)
        {
            case LogLevel.Debug:
                _debugEvents.Increment();
                break;
            case LogLevel.Info:
                _infoEvents.Increment();
                break;
            case LogLevel.Warn:
                _warnEvents.Increment();
                break;
            case LogLevel.Error:
                _errorEvents.Increment();
                break;
            case LogLevel.Fatal:
                _fatalEvents.Increment();
                break;
            default:
                return;
        }
    }

    public bool IsEnabledFor(LogLevel level) => true;
    public ILog ForContext(string context) => this;

    public LogLevelCounters Collect() =>
        new()
        {
            Debug = _debugEvents.CollectAndReset(),
            Info = _infoEvents.CollectAndReset(),
            Warn = _warnEvents.CollectAndReset(),
            Error = _errorEvents.CollectAndReset(),
            Fatal = _fatalEvents.CollectAndReset()
        };

    private sealed class ConcurrentCounter
    {
        private volatile int _value;

        public void Increment() => Interlocked.Increment(ref _value);
        public int CollectAndReset() => Interlocked.Exchange(ref _value, 0);
    }
}