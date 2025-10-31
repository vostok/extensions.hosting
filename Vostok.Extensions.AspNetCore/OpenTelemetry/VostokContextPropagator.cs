using System.Diagnostics;

using Microsoft.Extensions.Logging;

using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace Vostok.Extensions.AspNetCore.OpenTelemetry;

/// <summary>
/// <see cref="TextMapPropagator"/> implementation which extracts tracing context from `Context-Globals` header.
/// </summary>
internal sealed class VostokContextPropagator : TextMapPropagator
{
    internal const string ContextGlobalsHeader = "Context-Globals";
    internal const string XKonturTraceId = "X-Kontur-Trace-Id";
    internal const string XKonturTraceSpanId = "X-Kontur-Trace-SpanId";

    private readonly VostokContextReader _vostokContextReader;

    public static void Use(ILoggerFactory loggerFactory)
    {
        Sdk.SetDefaultTextMapPropagator(new CompositeTextMapPropagator(
        [
            Propagators.DefaultTextMapPropagator,
            new VostokContextPropagator(loggerFactory)
        ]));
    }

    internal VostokContextPropagator(ILoggerFactory loggerFactory)
    {
        _vostokContextReader = new VostokContextReader(loggerFactory.CreateLogger<VostokContextPropagator>());

        Fields = new HashSet<string>
        {
            ContextGlobalsHeader,
            XKonturTraceId,
            XKonturTraceSpanId,
        };
    }

    public override void Inject<T>(PropagationContext context, T carrier, Action<T, string, string> setter)
    {
    }

    public override PropagationContext Extract<T>(PropagationContext context, T carrier, Func<T, string, IEnumerable<string>?> getter)
    {
        if (context.ActivityContext.IsValid())
            return context;

        if (carrier == null)
            return context;

        var globals = getter(carrier, ContextGlobalsHeader)?.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(globals))
        {
            var traceContext = _vostokContextReader.Read(globals);
            if (traceContext is null)
                return context;

            return CreatePropagationContext<T>(context, traceContext.TraceId, traceContext.SpanId);
        }
        
        var traceIdString = getter(carrier, XKonturTraceId)?.FirstOrDefault();
        if (Guid.TryParse(traceIdString, out var legacyTraceId))
        {
            var spanIdString = getter(carrier, XKonturTraceSpanId)?.FirstOrDefault();
            var spanIdFromPrefix = LegacyTraceIdHelper.ParseSpanIdPrefix(spanIdString);
            var spanId = spanIdFromPrefix ?? Guid.NewGuid();
            
            
            return CreatePropagationContext<T>(context, legacyTraceId, spanId);
        }


        return context;
    }

    private static PropagationContext CreatePropagationContext<T>(PropagationContext context, Guid traceId, Guid spanId)
    {
        var activityTraceId = ActivityTraceId.CreateFromString(traceId.ToString("N"));
        var activitySpanId = ActivitySpanId.CreateFromString(spanId.ToString("N").AsSpan()[..16]);
        return new PropagationContext(
            new ActivityContext(activityTraceId, activitySpanId, ActivityTraceFlags.Recorded, isRemote: true),
            context.Baggage);
    }

    public override ISet<string> Fields { get; }
}