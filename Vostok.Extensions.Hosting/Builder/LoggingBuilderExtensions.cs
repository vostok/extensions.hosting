using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

using Vostok.Extensions.Hosting.Components.Logging;
using Vostok.Extensions.Hosting.Components.Logging.Builder;
using Vostok.Extensions.Hosting.Components.Logging.Substitution;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Microsoft;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class LoggingBuilderExtensions
{
    public const string ActualLogKey = "_ActualLog";

    public static IVostokBuilder WithLogging(this IVostokBuilder builder, Action<IVostokLogBuilder> configure)
    {
        var descriptor = ServiceDescriptor.Singleton<ILoggerProvider, VostokLoggerProvider>(VostokLogProviderFactory.Create);
        builder.Services.TryAddEnumerable(descriptor);

        var vostokLogBuilder = new VostokLogBuilder(builder.Services);
        configure(vostokLogBuilder);

        return builder;
    }

    internal static void EnableOuterVostokLog(this IVostokBuilder builder)
    {
        builder.Services.TryAddSingleton<ILog, SubstitutableLog>();
        builder.Services.TryAddKeyedSingleton<ILog>(ActualLogKey, (provider, _) =>
        {
            var microsoftLogger = provider.GetRequiredService<ILoggerFactory>()
                                          .CreateLogger(string.Empty);

            return new MicrosoftLog(microsoftLogger, new MicrosoftLogSettings());
        });
    }
}