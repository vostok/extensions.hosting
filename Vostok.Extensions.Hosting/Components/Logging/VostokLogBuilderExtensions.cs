using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.Components.Logging.Console;
using Vostok.Extensions.Hosting.Components.Logging.File;
using Vostok.Extensions.Hosting.Components.Logging.Internal;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Configuration;
using Vostok.Logging.Console;
using Vostok.Logging.File;
using Vostok.Logging.File.Configuration;

namespace Vostok.Extensions.Hosting.Components.Logging;

[PublicAPI]
public static class VostokLogBuilderExtensions
{
    public static IVostokLogBuilder AddLog(this IVostokLogBuilder builder, ILog log, string? logName = null, Action? customDispose = null)
    {
        builder.AddLog(_ => log, logName, customDispose);
        return builder;
    }

    public static IVostokLogBuilder AddLog(this IVostokLogBuilder builder, Func<IServiceProvider, ILog> implementationFactory, string? logName, Action? customDispose)
    {
        logName ??= LogRegistrationFactory.GetNextName();
        var logRegistration = LogRegistration.For(logName);

        builder.Services.TryAddKeyedSingleton(logRegistration, logRegistration);
        builder.Services.TryAddKeyedSingleton(
            logRegistration,
            (provider, _) => LogRegistrationFactory.Register(provider, implementationFactory, logName, customDispose));

        return builder;
    }

    public static IVostokLogBuilder AddConsole(
        this IVostokLogBuilder builder,
        Action<ConsoleLogSettings>? configure = null)
    {
        builder.AddLog(ConsoleLogFactory.Create, ConsoleLogFactory.LogName, ConsoleLog.Flush);

        if (configure is not null)
            builder.Services.Configure(configure);

        return builder;
    }

    public static IVostokLogBuilder AddFile(
        this IVostokLogBuilder builder,
        Action<FileLogSettings>? configureSettings = null)
    {
        builder.AddLog(FileLogFactory.Create, FileLogFactory.LogName, FileLog.FlushAll);

        if (configureSettings is not null)
            builder.Services.Configure(configureSettings);

        return builder;
    }

    public static IVostokLogBuilder CustomizeLog(
        this IVostokLogBuilder builder,
        string? name,
        Func<ILog, ILog> customize)
    {
        builder.CustomizeLog(name, (log, _) => customize(log));

        return builder;
    }

    public static IVostokLogBuilder CustomizeLog(
        this IVostokLogBuilder builder,
        string? name,
        Func<ILog, IServiceProvider, ILog> customize)
    {
        var customization = new LogCustomization(customize);

        if (name is null)
            builder.Services.AddSingleton(customization);
        else
            builder.Services.AddKeyedSingleton(name, customization);

        return builder;
    }

    public static IVostokLogBuilder ClearLogs(this IVostokLogBuilder builder)
    {
        builder.Services.RemoveAllKeyed<LogRegistration>(LogRegistration.All);
        builder.Services.RemoveAllKeyed<ILog>(LogRegistration.All);
        return builder;
    }

    public static IVostokLogBuilder DisableLog(this IVostokLogBuilder builder, string name)
    {
        builder.Services.RemoveAllKeyed<LogRegistration>(LogRegistration.For(name));
        builder.Services.RemoveAllKeyed<ILog>(LogRegistration.For(name));
        return builder;
    }

    public static IVostokLogBuilder AddRule(this IVostokLogBuilder builder, LogConfigurationRule rule)
    {
        builder.Services.Configure<VostokLogRulesOptions>(options => options.Rules.Add(rule));
        return builder;
    }

    public static IVostokLogBuilder ClearRules(this IVostokLogBuilder builder)
    {
        builder.Services.Configure<VostokLogRulesOptions>(options => options.Rules.Clear());
        return builder;
    }
}