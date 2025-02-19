using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Logging.Builder;

internal sealed class LogRegistration : IDisposable
{
    private readonly ILog? _log;
    private readonly string? _name;
    private readonly IDisposable? _logDisposing;

    public ILog Log => _log ?? throw new InvalidOperationException("Log is null");
    public string Name => _name ?? throw new InvalidOperationException("Name is null");

    public bool IsEnabled => _log is not null && _name is not null;

    public static readonly LogRegistration NullRegistration = new(null);
    private static int _unnamedLogsCounter;

    public LogRegistration(ILog? log, string? name = null, IDisposable? logDisposing = null)
    {
        _log = log;

        _name = name;
        if (_name is null && _log is not null)
            _name = $"{_log.GetType().Name}-{Interlocked.Increment(ref _unnamedLogsCounter)}";

        _logDisposing = logDisposing;

        if (_logDisposing is null && _log is IDisposable disposableLog)
            _logDisposing = disposableLog;
    }

    public void Dispose() => _logDisposing?.Dispose();
}