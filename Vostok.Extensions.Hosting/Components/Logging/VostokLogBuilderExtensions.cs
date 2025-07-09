using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.Components.Logging.Console;
using Vostok.Extensions.Hosting.Components.Logging.File;
using Vostok.Extensions.Hosting.Components.Logging.Internal;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Console;
using Vostok.Logging.File.Configuration;

namespace Vostok.Extensions.Hosting.Components.Logging;

[PublicAPI]
public static class VostokLogBuilderExtensions
{
    public static ILogConfigurator AddLog(
        this IVostokLogBuilder builder,
        ILog log,
        string? logName = null) =>
        builder.AddLog(_ => log, logName);

    public static ILogConfigurator AddLog(this IVostokLogBuilder builder,
        Func<IServiceProvider, ILog> implementationFactory,
        string? logName = null)
    {
        logName ??= LogRegistration.GetNextName();
        var key = LogRegistration.Key.For(logName);

        builder.Services.TryAddKeyedSingleton(key, new LogRegistration(logName, implementationFactory));

        return new LogConfigurator(builder.Services, logName);
    }

    public static ILogConfigurator WithConsole(
        this IVostokLogBuilder builder,
        Action<ConsoleLogSettings>? configureSettings = null)
    {
        if (configureSettings is not null)
            builder.Services.Configure(configureSettings);

        return builder.AddLog(ConsoleLogFactory.Create, ConsoleLogFactory.LogName);
    }

    public static ILogConfigurator WithFile(
        this IVostokLogBuilder builder,
        Action<FileLogSettings>? configureSettings = null)
    {
        if (configureSettings is not null)
            builder.Services.Configure(configureSettings);

        return builder.AddLog(FileLogFactory.Create, FileLogFactory.LogName);
    }

    public static ILogConfigurator Configure(this IVostokLogBuilder builder) =>
        new LogConfigurator(builder.Services, null);

    public static IVostokLogBuilder ClearLogs(this IVostokLogBuilder builder)
    {
        builder.Services.RemoveAllKeyed<LogRegistration>(LogRegistration.Key.All);
        return builder;
    }

    public static IVostokLogBuilder DisableLog(this IVostokLogBuilder builder, string name)
    {
        builder.Services.RemoveAllKeyed<LogRegistration>(LogRegistration.Key.For(name));
        return builder;
    }
}