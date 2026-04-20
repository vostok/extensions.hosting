using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Logging.Internal;

internal sealed class VostokLogBuilder(IServiceCollection services) : IVostokLogBuilder
{
    public IServiceCollection Services { get; } = services;

    public ILogConfigurator AddLog(Func<IServiceProvider, ILog> implementationFactory, string? logName = null)
    {
        logName ??= LogRegistration.GetNextName();

        Services.TryAddKeyedSingleton(logName, new LogRegistration(logName, implementationFactory));

        return new LogConfigurator(Services, logName);
    }

    public IVostokLogBuilder CustomizeLog(Func<ILog, IServiceProvider, ILog> customize)
    {
        // note: `null` name is for main log.
        var configurator = new LogConfigurator(Services, null);
        configurator.CustomizeLog(customize);

        return this;
    }

    public IVostokLogBuilder DisableLog(string name)
    {
        Services.RemoveAllKeyed<LogRegistration>(name);
        return this;
    }

    public IVostokLogBuilder ClearLogs()
    {
        var logsToRemove = Services
            .Where(d => d.ServiceType == typeof(LogRegistration))
            .ToList();

        foreach (var descriptor in logsToRemove)
            Services.Remove(descriptor);

        return this;
    }
}