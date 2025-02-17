using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

using Vostok.Extensions.Hosting.Components.Logging;
using Vostok.Extensions.Hosting.Components.Logging.Substitution;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Microsoft;

namespace Vostok.Extensions.Hosting.Builder;

internal static class LoggingBuilderExtensions
{
    public const string ActualLogKey = "_ActualLog";

    public static void WithLogging(this IVostokBuilder builder)
    {
        builder.Services.TryAddSingleton<ILog, SubstitutableLog>();
        builder.Services.TryAddKeyedSingleton<ILog>(ActualLogKey, (provider, _) =>
        {
            var logger = provider.GetRequiredService<ILoggerFactory>()
                                 .CreateLogger("Vostok.Log");

            return new MicrosoftLog(logger, new MicrosoftLogSettings());
        });

        builder.Services.AddLogEnricher<OperationContextEnricher>();
        builder.Services.AddStaticLogEnricher<ApplicationInfoEnricher>();
    }
}