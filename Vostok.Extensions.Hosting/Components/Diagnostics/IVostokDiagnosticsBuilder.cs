using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

[PublicAPI]
public interface IVostokDiagnosticsBuilder
{
    IServiceCollection Services { get; }
}