using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

/// <summary>
/// Builder for configuring Vostok diagnostics components.
/// </summary>
[PublicAPI]
public interface IVostokDiagnosticsBuilder
{
    /// <summary>
    /// The host <see cref="IServiceCollection"/> where Vostok diagnostics components are configured.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Registers provided <see cref="IDiagnosticInfoProvider"/>.
    /// </summary>
    /// <param name="entry"><see cref="DiagnosticEntry"/> used for <see cref="IDiagnosticInfoProvider"/> registration.</param>
    /// <param name="implementationFactory">The <see cref="IDiagnosticInfoProvider"/> implementation factory.</param>
    /// <returns><see cref="IVostokDiagnosticsBuilder"/> for chain method calls.</returns>
    IVostokDiagnosticsBuilder AddDiagnosticInfoProvider(
        DiagnosticEntry entry,
        Func<IServiceProvider, IDiagnosticInfoProvider> implementationFactory);
}