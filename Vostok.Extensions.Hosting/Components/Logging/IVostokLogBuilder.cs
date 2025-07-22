using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Logging;

[PublicAPI]
public interface IVostokLogBuilder
{
    IServiceCollection Services { get; }

    ILogConfigurator AddLog(Func<IServiceProvider, ILog> implementationFactory, string? logName = null);
    IVostokLogBuilder CustomizeLog(Func<ILog, IServiceProvider, ILog> customize);
    IVostokLogBuilder DisableLog(string name);
    IVostokLogBuilder ClearLogs();
}