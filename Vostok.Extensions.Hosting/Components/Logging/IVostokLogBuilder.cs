using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Logging;

/// <summary>
/// Builder for configuring Vostok logging components.
/// </summary>
[PublicAPI]
public interface IVostokLogBuilder
{
    /// <summary>
    /// The host <see cref="IServiceCollection"/> where Vostok logging components are configured.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Registers provided log in Vostok logging services.
    /// </summary>
    /// <param name="implementationFactory">The <see cref="ILog"/> implementation factory.</param>
    /// <param name="logName">Name for provided log. Will be generated automatically if not provided.</param>
    /// <returns><see cref="ILogConfigurator"/> for configure registered log</returns>
    ILogConfigurator AddLog(Func<IServiceProvider, ILog> implementationFactory, string? logName = null);

    /// <summary>
    /// Customizes main <see cref="ILog"/>.
    /// </summary>
    /// <param name="customize">Delegate to customize main <see cref="ILog"/>.</param>
    /// <returns><see cref="IVostokLogBuilder"/> for chain method calls.</returns>
    IVostokLogBuilder CustomizeLog(Func<ILog, IServiceProvider, ILog> customize);

    /// <summary>
    /// Disables log registration with given <paramref name="name"/>.
    /// </summary>
    /// <param name="name">Log name to disable.</param>
    /// <returns><see cref="IVostokLogBuilder"/> for chain method calls.</returns>
    IVostokLogBuilder DisableLog(string name);

    /// <summary>
    /// Disables all logging registrations.
    /// </summary>
    /// <returns><see cref="IVostokLogBuilder"/> for chain method calls.</returns>
    IVostokLogBuilder ClearLogs();
}