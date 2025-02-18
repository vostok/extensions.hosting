using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Logging.Builder;

internal sealed class LogRegistration : IDisposable
{
    public ILog? Log { get; }
    private readonly IDisposable? _logDisposing;

    public static readonly LogRegistration NullRegistration = new(null);

    public LogRegistration(ILog? log, IDisposable? logDisposing = null)
    {
        Log = log;
        _logDisposing = logDisposing;

        if (_logDisposing is null && Log is IDisposable disposableLog)
            _logDisposing = disposableLog;
    }

    public void Dispose() => _logDisposing?.Dispose();
}