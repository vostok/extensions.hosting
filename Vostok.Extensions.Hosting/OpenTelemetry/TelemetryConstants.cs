using Vostok.Tracing.Diagnostics;

namespace Vostok.Extensions.Hosting.OpenTelemetry;

internal static class TelemetryConstants
{
    public const string VostokLogMeterName = "VostokLog";
    public const string VostokTracerActivitySourceName = TracingConstants.VostokTracerActivitySourceName;
}