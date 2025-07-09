using JetBrains.Annotations;

using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Logging;

[PublicAPI]
public interface ILogConfigurator
{
    ILogConfigurator SetMinimumLevelProvider(Func<LogLevel> minLevelProvider);
    ILogConfigurator CustomizeLog(Func<ILog, IServiceProvider, ILog> customize);
}