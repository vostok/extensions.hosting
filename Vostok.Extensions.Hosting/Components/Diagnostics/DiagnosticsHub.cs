using Vostok.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

internal class DiagnosticsHub(IDiagnosticInfo info) : IVostokApplicationDiagnostics, IDisposable
{
    public IDiagnosticInfo Info { get; } = info;
    /*public HealthTracker HealthTracker { get; } = HealthTracker;*/
    
    public void Dispose()
    {
        /* HealthTracker.Dispose(); */
    }

    IDiagnosticInfo IVostokApplicationDiagnostics.Info => Info;
    IHealthTracker IVostokApplicationDiagnostics.HealthTracker => null!;
}