using Vostok.Commons.Time;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.Hosting.Components.Diagnostics.InfoProviders;

internal sealed class HealthChecksInfoProvider(IHealthTracker healthTracker) : IDiagnosticInfoProvider
{
    public object Query()
    {
        var report = healthTracker.CurrentReport;

        return new
        {
            CurrentStatus = report.Status,
            Duration = report.Duration.ToPrettyString(),
            Timestamp = report.Timestamp.ToString("O"),
            Recency = (DateTime.UtcNow - report.Timestamp).ToPrettyString() + " ago",
            report.Problems,
            report.Checks
        };
    }
}