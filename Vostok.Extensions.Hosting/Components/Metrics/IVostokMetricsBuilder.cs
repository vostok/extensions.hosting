using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Components.Metrics;

[PublicAPI]
public interface IVostokMetricsBuilder
{
    IServiceCollection Services { get; }
}