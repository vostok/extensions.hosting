using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.Components.Logging.Console;
using Vostok.Extensions.Hosting.Components.Logging.File;
using Vostok.Extensions.Hosting.Components.Logging.Internal;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Configuration;
using Vostok.Logging.Console;
using Vostok.Logging.File.Configuration;

namespace Vostok.Extensions.Hosting.Components.Logging;

[PublicAPI]
public static class VostokLogBuilderExtensions
{
    public static IVostokLogBuilder AddLog(this IVostokLogBuilder builder, ILog log, string? logName = null) =>
        builder.AddLog(_ => log, logName);

    public static IVostokLogBuilder AddLog(
        this IVostokLogBuilder builder,
        Func<IServiceProvider, ILog> implementationFactory,
        string? logName = null)
    {
        logName ??= LogRegistration.GetNextName();
        var key = LogRegistration.Key.For(logName);

        builder.Services.TryAddKeyedSingleton(key, new LogRegistration(logName, implementationFactory));
        return builder;
    }

    public static IVostokLogBuilder AddConsole(
        this IVostokLogBuilder builder,
        Action<ConsoleLogSettings>? configure = null)
    {
        builder.AddLog(ConsoleLogFactory.Create, ConsoleLogFactory.LogName);

        if (configure is not null)
            builder.Services.Configure(configure);

        return builder;
    }

    public static IVostokLogBuilder AddFile(
        this IVostokLogBuilder builder,
        Action<FileLogSettings>? configureSettings = null)
    {
        builder.AddLog(FileLogFactory.Create, FileLogFactory.LogName);

        if (configureSettings is not null)
            builder.Services.Configure(configureSettings);

        return builder;
    }

    /// <summary>
    /// Enables file log with default settings.
    /// </summary>
    public static IVostokLogBuilder AddFileWithDefaults(this IVostokLogBuilder builder) =>
        builder.AddFile(settings =>
        {
            settings.RollingStrategy.Type = RollingStrategyType.ByTime;
            settings.RollingStrategy.Period = RollingPeriod.Day;
        });

    public static IVostokLogBuilder WithMinimumLevelProvider(
        this IVostokLogBuilder builder,
        string name,
        Func<LogLevel> minLevelProvider)
    {
        builder.Services.Configure<VostokLogMiscellaneousOptions>(name,
            options => options.MinLevelProvider = minLevelProvider);

        return builder;
    }

    public static IVostokLogBuilder CustomizeLog(this IVostokLogBuilder builder, Func<ILog, ILog> customize) =>
        builder.CustomizeLog((log, _) => customize(log));

    public static IVostokLogBuilder CustomizeLog(
        this IVostokLogBuilder builder,
        Func<ILog, IServiceProvider, ILog> customize)
    {
        var customization = new LogCustomization(customize);
        builder.Services.Configure<VostokLogMiscellaneousOptions>(options => options.Customizations.Add(customization));

        return builder;
    }

    public static IVostokLogBuilder CustomizeLog(
        this IVostokLogBuilder builder,
        string name,
        Func<ILog, ILog> customize) =>
        builder.CustomizeLog(name, (log, _) => customize(log));

    public static IVostokLogBuilder CustomizeLog(
        this IVostokLogBuilder builder,
        string name,
        Func<ILog, IServiceProvider, ILog> customize)
    {
        var customization = new LogCustomization(customize);
        builder.Services.Configure<VostokLogMiscellaneousOptions>(name,
            options => options.Customizations.Add(customization));

        return builder;
    }

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