using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Logging.Builder;

[PublicAPI]
public static class VostokLogBuilderExtensions
{
    public static IVostokLogBuilder AddLog(this IVostokLogBuilder builder, ILog log)
    {
        builder.Services.AddSingleton(new LogRegistration(log));
        return builder;
    }

    public static IVostokLogBuilder AddConsole(
        this IVostokLogBuilder builder,
        Action<ConsoleLogOptions>? configure = null)
    {
        builder.Services.AddSingleton(ConsoleLogFactory.Create);

        if (configure is not null)
            builder.Services.Configure(configure);

        return builder;
    }

    public static IVostokLogBuilder AddFile(
        this IVostokLogBuilder builder,
        Action<FileLogOptions>? configure = null)
    {
        builder.Services.AddSingleton(FileLogFactory.Create);

        if (configure is not null)
            builder.Services.Configure(configure);

        return builder;
    }

    public static IVostokLogBuilder ClearLogs(this IVostokLogBuilder builder)
    {
        builder.Services.RemoveAll<LogRegistration>();
        return builder;
    }
}