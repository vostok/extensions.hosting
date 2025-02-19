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

        ConfigureVostokDefaults(builder);
    }

    public static void WithLogging(this IVostokBuilder builder, Action<IVostokLogBuilder> configure)
    {
        var descriptor = ServiceDescriptor.Singleton<ILoggerProvider, VostokLoggerProvider>(VostokLogProviderFactory.Create);
        builder.Services.TryAddEnumerable(descriptor);

        var vostokLogBuilder = new VostokLogBuilder(builder.Services);
        configure(vostokLogBuilder);
    }

    private static void ConfigureVostokDefaults(IVostokBuilder builder)
    {
        // todo: protect from multiple invocation
        builder.Services.AddLogLevelCounting();

        builder.Services.AddOptions<LogConfigurationOptions>()
               .Configure<IServiceProvider>((options, provider) =>
               {
                   options.CustomizeFinalLog(log => log.WithVostokSpecificProperties(provider));
               });
    }

    private static void AddLogLevelCounting(this IServiceCollection services)
    {
        services.TryAddSingleton<LogLevelMetrics>();

        var levelCountingLog = new LevelCountingLog();
        services.AddSingleton(new LogRegistration(levelCountingLog, "_CounterLog"));
        services.TryAddSingleton<ILogLevelCountersCollector>(levelCountingLog);
        services.AddHostedService<LogLevelMetricsHostedService>();
    }
}