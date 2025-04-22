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
            ContextGlobalsHeader
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
        if (string.IsNullOrWhiteSpace(globals))
            return context;

        var traceContext = _vostokContextReader.Read(globals);
        if (traceContext is null)
            return context;

        var traceId = ActivityTraceId.CreateFromString(traceContext.TraceId.ToString("N"));
        var spanId = ActivitySpanId.CreateFromString(traceContext.SpanId.ToString("N").AsSpan()[..16]);
        return new PropagationContext(
            new ActivityContext(traceId, spanId, ActivityTraceFlags.Recorded, isRemote: true),
            context.Baggage);
    }

    public override ISet<string> Fields { get; }
}