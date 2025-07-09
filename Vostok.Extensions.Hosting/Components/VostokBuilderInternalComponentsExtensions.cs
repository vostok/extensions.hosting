using System.Diagnostics;

using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.Components.Logging.Internal;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.Logging.Abstractions;
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
        vostok.Services.TryAddSingleton(VostokLogFactory.CreateVostokLogAdapter);
    }
}