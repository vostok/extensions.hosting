using Vostok.Commons.Time;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.Hosting.Components.Diagnostics.InfoProviders;

internal sealed class HealthChecksInfoProvider(IHealthTracker healthTracker) : IDiagnosticInfoProvider
{
    public object Query()
    {
        var report = healthTracker.CurrentReport;
        var problems = report.GetProblems();
        var timestamp = healthTracker.CurrentReportTimestamp;

        return new
        {
            CurrentStatus = report.Status,
            Duration = report.TotalDuration.ToPrettyString(),
            Timestamp = timestamp.ToString("O"),
            Recency = (DateTime.UtcNow - timestamp).ToPrettyString() + " ago",
            problems,
            report.Entries
        };
    }
}