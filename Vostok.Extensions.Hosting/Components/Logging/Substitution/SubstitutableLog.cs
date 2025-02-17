using Vostok.Logging.Abstractions;
using Vostok.Logging.Abstractions.Wrappers;

namespace Vostok.Extensions.Hosting.Components.Logging.Substitution;

public class SubstitutableLog : ILog
{
    private ILog _baseLog = new BufferedLog();

    public bool IsBuffering => _baseLog is BufferedLog;

    public void Log(LogEvent @event) => _baseLog.Log(@event);

    public bool IsEnabledFor(LogLevel level) => _baseLog.IsEnabledFor(level);

    public ILog ForContext(string context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        return new SourceContextWrapper(this, context);
    }

    public void SubstituteWith(ILog newLog)
    {
        if (_baseLog is BufferedLog bufferedLog)
        {
            bufferedLog.SendBufferedEvents(newLog);

            _baseLog = newLog;

            // note (kungurtsev, 31.08.2021): hack to avoid deadlock between settings logging and using
            bufferedLog.SendBufferedEvents(newLog);
        }
        else
        {
            _baseLog = newLog;
        }
    }
}