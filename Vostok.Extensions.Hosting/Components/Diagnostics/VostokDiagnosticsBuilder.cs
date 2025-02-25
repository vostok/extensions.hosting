using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

internal sealed class VostokDiagnosticsBuilder(IServiceCollection services) : IVostokDiagnosticsBuilder
{
    public IServiceCollection Services { get; } = services;
}