using Vostok.Tracing.Abstractions;

namespace Vostok.Extensions.AspNetCore.OpenTelemetry;

internal class TraceContextSerializer
{
    private static readonly char[] DelimiterArray = [';'];

    public TraceContext Deserialize(string input)
    {
        var parts = input.Split(DelimiterArray, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2 ||
            !Guid.TryParse(parts[0], out var traceId) ||
            !Guid.TryParse(parts[1], out var spanId))
        {
            throw new FormatException($"Failed to parse {nameof(TraceContext)} from following input: '{input}'.");
        }

        return new TraceContext(traceId, spanId);
    }
}