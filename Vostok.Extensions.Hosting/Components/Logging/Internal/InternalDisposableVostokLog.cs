using System.Runtime.CompilerServices;

using Vostok.Logging.Abstractions;
using Vostok.Logging.Console;
using Vostok.Logging.File;

namespace Vostok.Extensions.Hosting.Components.Logging.Internal;

internal sealed class InternalDisposableVostokLog(ILog baseLog) : ILog, IDisposable
{
    private bool _disposed;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Log(LogEvent? @event)
    {
        if (_disposed)
            return;

        baseLog.Log(@event);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsEnabledFor(LogLevel level) => !_disposed && baseLog.IsEnabledFor(level);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ILog ForContext(string context) => baseLog.ForContext(context);

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        FileLog.FlushAll();
        ConsoleLog.Flush();
    }
}