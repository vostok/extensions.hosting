using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Vostok.Extensions.Hosting.Components.Logging.Console;
using Vostok.Extensions.Hosting.Components.Logging.File;
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

    public static IVostokLogBuilder CustomizeLog(this IVostokLogBuilder builder, Func<ILog, ILog> customize) =>
        builder.CustomizeLog((log, _) => customize(log));

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
}