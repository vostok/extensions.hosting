using Vostok.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Diagnostics;

using ILegacyHealthTracker = Vostok.Hosting.Abstractions.Diagnostics.IHealthTracker;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

internal sealed class DiagnosticsHub(IDiagnosticInfo info, ILegacyHealthTracker healthTracker) : IVostokApplicationDiagnostics
{
    public IDiagnosticInfo Info { get; } = info;
    public ILegacyHealthTracker HealthTracker { get; } = healthTracker;
}