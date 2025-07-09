using JetBrains.Annotations;

using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Logging;

[PublicAPI]
public static class LogConfiguratorExtensions
{
    public static ILogConfigurator CustomizeLog(
        this ILogConfigurator configurator,
        Func<ILog, ILog> customize) =>
        configurator.CustomizeLog((log, _) => customize(log));
}