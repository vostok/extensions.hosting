using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.Hosting.Components.Diagnostics.DiagnosticInfo;

internal sealed class VostokDiagnosticsOptions
{
    public ICollection<VostokDiagnosticInfoRegistration> Registrations { get; } = [];
}

internal sealed class VostokDiagnosticInfoRegistration(
    DiagnosticEntry entry,
    Func<IServiceProvider, IDiagnosticInfoProvider> implementationFactory)
{
    public VostokDiagnosticInfoRegistration(DiagnosticEntry entry, IDiagnosticInfoProvider provider)
        : this(entry, _ => provider)
    {
    }

    public DiagnosticEntry Entry { get; } = entry;
    public Func<IServiceProvider, IDiagnosticInfoProvider> ImplementationFactory { get; } = implementationFactory;
}