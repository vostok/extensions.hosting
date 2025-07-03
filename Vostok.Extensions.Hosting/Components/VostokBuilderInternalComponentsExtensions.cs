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
        var initializationLog = InitializationLog.Get();
        LogProvider.Configure(initializationLog, true);

        vostok.Services.TryAddKeyedSingleton<ILog>(InitializationLog.Key, initializationLog);
        vostok.Services.TryAddSingleton<ILog>(provider =>
        {
            var microsoftLogger = provider.GetRequiredService<ILoggerFactory>()
                                          .CreateLogger(string.Empty);

            return new MicrosoftLog(microsoftLogger, new MicrosoftLogSettings());
        });
    }
}