using Vostok.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

internal sealed class DiagnosticsHub(IDiagnosticInfo info, IHealthTracker healthTracker) : IVostokApplicationDiagnostics
{
    public IDiagnosticInfo Info { get; } = info;
    public IHealthTracker HealthTracker { get; } = healthTracker;
}