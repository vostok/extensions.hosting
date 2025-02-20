using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.Components.Logging.Console;
using Vostok.Extensions.Hosting.Components.Logging.File;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Console;
using Vostok.Logging.File.Configuration;

namespace Vostok.Extensions.Hosting.Components.Logging.Builder;

[PublicAPI]
public static class VostokLogBuilderExtensions
{
    public static IVostokLogBuilder AddLog(this IVostokLogBuilder builder, ILog log)
    {
        builder.Services.AddKeyedSingleton(LogRegistration.Unnamed, log);
        return builder;
    }

    public static IVostokLogBuilder AddLog(
        this IVostokLogBuilder builder,
        Func<IServiceProvider, ILog> implementationFactory)
    {
        builder.Services.AddKeyedSingleton(LogRegistration.Unnamed, implementationFactory);
        return builder;
    }

    public static IVostokLogBuilder AddConsole(
        this IVostokLogBuilder builder,
        Action<ConsoleLogSettings>? configure = null)
    {
        builder.Services.TryAddKeyedSingleton(LogRegistration.For(ConsoleLogFactory.LogName), ConsoleLogFactory.Create);

        if (configure is not null)
            builder.Services.Configure(configure);

        return builder;
    }

    public static IVostokLogBuilder AddFile(
        this IVostokLogBuilder builder,
        Action<FileLogSettings>? configureSettings = null)
    {
        builder.Services.TryAddKeyedSingleton(LogRegistration.For(FileLogFactory.LogName), FileLogFactory.Create);

        if (configureSettings is not null)
            builder.Services.Configure(configureSettings);

        return builder;
    }

    public static IVostokLogBuilder ClearLogs(this IVostokLogBuilder builder)
    {
        builder.Services.RemoveAllKeyed<ILog>(LogRegistration.All);
        return builder;
    }

    public static IVostokLogBuilder CustomizeLog(
        this IVostokLogBuilder builder,
        Func<ILog, ILog> customize,
        string? name = null)
    {
        builder.CustomizeLog((log, _) => customize(log), name);

        return builder;
    }

    public static IVostokLogBuilder CustomizeLog(
        this IVostokLogBuilder builder,
        Func<ILog, IServiceProvider, ILog> customize,
        string? name = null)
    {
        var customization = new LogCustomization(customize);

        if (name is null)
            builder.Services.AddSingleton(customization);

        builder.Services.AddKeyedSingleton(name, customization);

        return builder;
    }

    public static IVostokLogBuilder DisableLog(this IVostokLogBuilder builder, string name)
    {
        builder.Services.RemoveAllKeyed<ILog>(LogRegistration.For(name));
        return builder;
    }
}