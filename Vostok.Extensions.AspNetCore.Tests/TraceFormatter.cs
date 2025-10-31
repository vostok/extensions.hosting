namespace Vostok.Extensions.AspNetCore.Tests;

public static class TraceFormatter
{
    public static string FormatTraceId(Guid traceId) =>
        traceId.ToString("N");

    public static string FormatSpanId(Guid spanId) =>
        spanId.ToString("N")[..16];

    public static string FormatLegacySpanId(Guid spanId, bool withLeadingZeros = true) =>
        withLeadingZeros
            ? spanId.ToString("N")[..8] + new string('0', 8)
            : spanId.ToString("N")[..8];
}