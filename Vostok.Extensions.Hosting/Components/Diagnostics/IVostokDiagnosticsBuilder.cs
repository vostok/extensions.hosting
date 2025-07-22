using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

[PublicAPI]
public interface IVostokDiagnosticsBuilder
{
    IServiceCollection Services { get; }

    IVostokDiagnosticsBuilder AddDiagnosticInfoProvider(
        DiagnosticEntry entry,
        Func<IServiceProvider, IDiagnosticInfoProvider> implementationFactory);
}