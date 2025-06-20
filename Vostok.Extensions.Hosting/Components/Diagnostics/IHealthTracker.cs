using JetBrains.Annotations;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

/// <summary>
/// Provides a simple health check infrastructure and allows to track <see cref="HealthReport"/>s.
/// </summary>
[PublicAPI]
public interface IHealthTracker
{
    /// <summary>
    /// The latest published <see cref="HealthReport"/>.
    /// </summary>
    HealthReport CurrentReport { get; }

    /// <summary>
    /// Timestamp the <see cref="CurrentReport"/> was published.
    /// </summary>
    DateTimeOffset CurrentReportTimestamp { get; }

    /// <summary>
    /// Returns an observable that produces a new element each time a <see cref="HealthReport"/> is published.
    /// </summary>
    IObservable<HealthReport> ObserveReports();
}