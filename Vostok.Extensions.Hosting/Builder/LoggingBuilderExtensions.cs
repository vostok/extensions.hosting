using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

using Vostok.Extensions.Hosting.Components.Logging;
using Vostok.Extensions.Hosting.Components.Logging.Builder;
using Vostok.Extensions.Hosting.Components.Logging.LevelCounter;
using Vostok.Extensions.Hosting.Components.Logging.Substitution;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Context;
using Vostok.Logging.Microsoft;

namespace Vostok.Extensions.Hosting.Builder;

public static class LoggingBuilderExtensions
{
    public const string ActualLogKey = "_ActualLog";

    internal static void AddLogging(this IVostokBuilder builder)
    {
        builder.Services.TryAddSingleton<ILog, SubstitutableLog>();
        builder.Services.TryAddKeyedSingleton<ILog>(ActualLogKey, (provider, _) =>
        {
            var microsoftLogger = provider.GetRequiredService<ILoggerFactory>()
                                          .CreateLogger("Vostok.Log");

            var vostokLogAdapter = new MicrosoftLog(microsoftLogger, new MicrosoftLogSettings());
            return vostokLogAdapter.WithOperationContext();
        });

        builder.WithLogging(logBuilder =>
        {
            // todo: protect from multiple invocation
            logBuilder.AddLogLevelCounting();
            logBuilder.CustomizeLog((log, provider) => log.WithVostokSpecificProperties(provider));
        });
    }

    public static IVostokBuilder WithLogging(this IVostokBuilder builder, Action<IVostokLogBuilder> configure)
    {
        var descriptor = ServiceDescriptor.Singleton<ILoggerProvider, VostokLoggerProvider>(VostokLogProviderFactory.Create);
        builder.Services.TryAddEnumerable(descriptor);

        var vostokLogBuilder = new VostokLogBuilder(builder.Services);
        configure(vostokLogBuilder);

        return builder;
    }

    private static void AddLogLevelCounting(this IVostokLogBuilder logBuilder)
    {
        logBuilder.Services.TryAddSingleton<LogLevelMetrics>();

        var levelCountingLog = new LevelCountingLog();
        logBuilder.AddLog(levelCountingLog);
        logBuilder.Services.TryAddSingleton<ILogLevelCountersCollector>(levelCountingLog);

        // todo: Enable later
        // services.AddHostedService<LogLevelMetricsHostedService>();
    }
}