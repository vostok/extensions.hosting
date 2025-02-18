using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

using Vostok.Extensions.Hosting.Components.Logging;
using Vostok.Extensions.Hosting.Components.Logging.Builder;
using Vostok.Extensions.Hosting.Components.Logging.Substitution;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Microsoft;

namespace Vostok.Extensions.Hosting.Builder;

public static class LoggingBuilderExtensions
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

    public static void WithLogging(this IVostokBuilder builder, Action<IVostokLogBuilder> configure)
    {
        var descriptor = ServiceDescriptor.Singleton<ILoggerProvider, VostokLoggerProvider>(VostokLogProviderFactory.Create);
        builder.Services.TryAddEnumerable(descriptor);

        var vostokLogBuilder = new VostokLogBuilder(builder.Services);
        configure(vostokLogBuilder);
    }
}