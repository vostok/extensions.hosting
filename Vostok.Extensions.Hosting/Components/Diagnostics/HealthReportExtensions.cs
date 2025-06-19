using JetBrains.Annotations;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

[PublicAPI]
public static class HealthReportExtensions
{
    public static IEnumerable<string> GetProblems(this HealthReport report) =>
        report.Entries.Values
              .Where(result => result.Status != HealthStatus.Healthy)
              .Where(result => result.Description != null)
              .Select(result => result.Description!);
}