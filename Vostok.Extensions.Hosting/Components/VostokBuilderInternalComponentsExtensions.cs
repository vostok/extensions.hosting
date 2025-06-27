using System.Diagnostics;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Microsoft;
using Vostok.Tracing.Abstractions;
using Vostok.Tracing.Diagnostics;

namespace Vostok.Extensions.Hosting.Components;

internal static class VostokBuilderInternalExtensions
{
    public const string ActualLogKey = "_ActualLog";

    public static void WithVostokTracerAdapter(this IVostokBuilder vostok)
    {
        var settings = new ActivitySourceTracerSettings
        {
            ActivitySource = new ActivitySource(TracingConstants.VostokTracerActivitySourceName)
        };

        var activitySourceTracer = new ActivitySourceTracer(settings);
        TracerProvider.Configure(activitySourceTracer, true);

        vostok.Services.TryAddSingleton<ITracer>(activitySourceTracer);
    }

    public static void WithVostokLogAdapter(this IVostokBuilder vostok)
    {
        var substitutableLog = InitializationLogProvider.Get();
        LogProvider.Configure(substitutableLog, true);

        vostok.Services.TryAddSingleton<ILog>(substitutableLog);
        vostok.Services.TryAddKeyedSingleton<ILog>(ActualLogKey, (provider, _) =>
        {
            var microsoftLogger = provider.GetRequiredService<ILoggerFactory>()
                                          .CreateLogger(string.Empty);

            return new MicrosoftLog(microsoftLogger, new MicrosoftLogSettings());
        });
    }
}