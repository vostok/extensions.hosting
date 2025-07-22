using Microsoft.Extensions.DependencyInjection;

using Vostok.Extensions.Hosting.Components.Diagnostics.DiagnosticInfo;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

internal sealed class VostokDiagnosticsBuilder(IServiceCollection services) : IVostokDiagnosticsBuilder
{
    public IServiceCollection Services { get; } = services;

    public IVostokDiagnosticsBuilder AddDiagnosticInfoProvider(
        DiagnosticEntry entry,
        Func<IServiceProvider, IDiagnosticInfoProvider> implementationFactory)
    {
        var registration = new VostokDiagnosticInfoRegistration(entry, implementationFactory);
        Services.Configure<VostokDiagnosticsOptions>(options => options.Registrations.Add(registration));

        return this;
    }
}