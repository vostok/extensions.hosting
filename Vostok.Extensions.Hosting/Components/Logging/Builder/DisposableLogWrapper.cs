using System.Runtime.CompilerServices;

using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Logging.Builder;

internal sealed class DisposableLogWrapper(ILog baseLog, IDisposable dispose) : ILog, IDisposable
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Log(LogEvent? @event) => baseLog.Log(@event);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsEnabledFor(LogLevel level) => baseLog.IsEnabledFor(level);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ILog ForContext(string context) => baseLog.ForContext(context);

    public void Dispose() => dispose.Dispose();
}