using Vostok.Commons.Collections;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Abstractions.Wrappers;

namespace Vostok.Extensions.Hosting.Components.Logging.Substitution;

internal class BufferedLog : ILog
{
    private const int Capacity = 1_000;
    private readonly ConcurrentBoundedQueue<LogEvent> _queue = new(Capacity);

    public void Log(LogEvent @event)
        => _queue.TryAdd(@event);

    public bool IsEnabledFor(LogLevel level) => true;

    public ILog ForContext(string context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        return new SourceContextWrapper(this, context);
    }

    public void SendBufferedEvents(ILog log)
    {
        var buffer = new LogEvent[_queue.Count];
        var count = _queue.Drain(buffer, 0, buffer.Length);
        for (var i = 0; i < count; i++)
            log.Log(buffer[i]);
    }
}