using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

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
    public static IVostokLogBuilder AddLog(
        this IVostokLogBuilder builder,
        ILog log,
        string? logName = null,
        Action<ILogConfigurator>? configure = null) =>
        builder.AddLog(_ => log, logName, configure);

    public static IVostokLogBuilder AddLog(this IVostokLogBuilder builder,
        Func<IServiceProvider, ILog> implementationFactory,
        string? logName = null,
        Action<ILogConfigurator>? configure = null)
    {
        logName ??= LogRegistration.GetNextName();
        var key = LogRegistration.Key.For(logName);

        builder.Services.TryAddKeyedSingleton(key, new LogRegistration(logName, implementationFactory));

        configure?.Invoke(new LogConfigurator(builder.Services, logName));

        return builder;
    }

    public static IVostokLogBuilder WithConsole(
        this IVostokLogBuilder builder,
        Action<ConsoleLogSettings>? configureSettings = null,
        Action<ILogConfigurator>? configureLog = null)
    {
        builder.AddLog(ConsoleLogFactory.Create, ConsoleLogFactory.LogName, configureLog);

        if (configureSettings is not null)
            builder.Services.Configure(configureSettings);

        return builder;
    }

    public static IVostokLogBuilder WithFile(
        this IVostokLogBuilder builder,
        Action<FileLogSettings>? configureSettings = null,
        Action<ILogConfigurator>? configureLog = null)
    {
        builder.AddLog(FileLogFactory.Create, FileLogFactory.LogName, configureLog);

        if (configureSettings is not null)
            builder.Services.Configure(configureSettings);

        return builder;
    }

    public static IVostokLogBuilder SetMinimumLevelProvider(
        this IVostokLogBuilder builder,
        Func<LogLevel> minLevelProvider)
    {
        builder.Services.ConfigureMiscellaneousOptions(options => options.MinLevelProvider = minLevelProvider);
        return builder;
    }

    public static IVostokLogBuilder CustomizeLog(this IVostokLogBuilder builder, Func<ILog, ILog> customize) =>
        builder.CustomizeLog((log, _) => customize(log));

    public static IVostokLogBuilder CustomizeLog(
        this IVostokLogBuilder builder,
        Func<ILog, IServiceProvider, ILog> customize)
    {
        var customization = new LogCustomization(customize);
        builder.Services.ConfigureMiscellaneousOptions(options => options.Customizations.Add(customization));

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

    internal static void ConfigureMiscellaneousOptions(
        this IServiceCollection services,
        Action<VostokLogMiscellaneousOptions> configure,
        string? name = null)
    {
        name ??= Options.DefaultName;
        services.Configure(name, configure);
    }
}