using System.Diagnostics;

using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.OpenTelemetry;
using Vostok.Tracing.Abstractions;
using Vostok.Tracing.Diagnostics;

namespace Vostok.Extensions.Hosting.Builder;

internal static class TracingBuilderExtensions
{
    public static void AddTracing(this IVostokBuilder builder)
    {
        var settings = new ActivitySourceTracerSettings
        {
            ActivitySource = new ActivitySource(TelemetryConstants.VostokTracerActivitySourceName)
        };

        var activitySourceTracer = new ActivitySourceTracer(settings);
        builder.Services.TryAddSingleton<ITracer>(activitySourceTracer);
    }
}