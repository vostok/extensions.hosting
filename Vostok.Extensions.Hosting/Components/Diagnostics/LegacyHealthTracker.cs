using System.Collections;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Components.Diagnostics.Helpers;

using HealthReport = Vostok.Hosting.Abstractions.Diagnostics.HealthReport;
using HealthStatus = Vostok.Hosting.Abstractions.Diagnostics.HealthStatus;
using IHealthCheck = Vostok.Hosting.Abstractions.Diagnostics.IHealthCheck;
using ILegacyHealthTracker = Vostok.Hosting.Abstractions.Diagnostics.IHealthTracker;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

internal sealed class LegacyHealthTracker(IOptions<HealthCheckServiceOptions> options) : ILegacyHealthTracker
{
    public IDisposable RegisterCheck(string name, IHealthCheck check)
    {
        var microsoftCheck = check.ToMicrosoftCheck();
        options.Value.Registrations.Add(VostokHealthCheckHelpers.RegisterWithVostokTag(name, microsoftCheck));

        return new EmptyDisposable();
    }

    public IEnumerator<(string name, IHealthCheck check)> GetEnumerator() => throw new InvalidOperationException(GetError());
    IEnumerator IEnumerable.GetEnumerator() => throw new InvalidOperationException(GetError());
    public IObservable<HealthStatus> ObserveStatus() => throw new InvalidOperationException(GetError());
    public IObservable<HealthReport> ObserveReports() => throw new InvalidOperationException(GetError());
    public IObservable<(HealthStatus previous, HealthStatus current)> ObserveStatusChanges() => throw new InvalidOperationException(GetError());
    public Task<HealthReport> RunChecksAsync(CancellationToken cancellationToken) => throw new InvalidOperationException(GetError());

    public HealthStatus CurrentStatus => throw new InvalidOperationException(GetError());
    public HealthReport CurrentReport => throw new InvalidOperationException(GetError());

    private static string GetError([CallerMemberName] string methodName = "") =>
        $"'{methodName}' not allowed for '{typeof(ILegacyHealthTracker).FullName}'. Only '{nameof(RegisterCheck)}' method implemented for backward compatibility purposes. Use .NET health checks infrastructure instead.";

    private sealed class EmptyDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
}