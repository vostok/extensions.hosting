using System.Globalization;

namespace Vostok.Extensions.AspNetCore.OpenTelemetry;

internal static class LegacyTraceIdHelper
{
    public static unsafe Guid? ParseSpanIdPrefix(string? s)
    {
        if (s == null || !int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var value))
            return null;

        var guid = Guid.Empty;
        *(int*)&guid = value;

        return guid;
    }
}