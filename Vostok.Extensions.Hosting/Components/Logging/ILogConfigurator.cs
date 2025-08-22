using JetBrains.Annotations;

using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Logging;

/// <summary>
/// Type for configuring registered <see cref="ILog"/>.
/// </summary>
[PublicAPI]
public interface ILogConfigurator
{
    /// <summary>
    /// Sets minimum level provider.
    /// </summary>
    /// <param name="minLevelProvider">Minimum <see cref="LogLevel"/> provider.</param>
    /// <returns><see cref="ILogConfigurator"/> for chain method calls.</returns>
    ILogConfigurator SetMinimumLevelProvider(Func<LogLevel> minLevelProvider);

    /// <summary>
    /// Customizes <see cref="ILog"/>.
    /// </summary>
    /// <param name="customize">Delegate to customize <see cref="ILog"/>.</param>
    /// <returns><see cref="ILogConfigurator"/> for chain method calls.</returns>
    ILogConfigurator CustomizeLog(Func<ILog, IServiceProvider, ILog> customize);
}