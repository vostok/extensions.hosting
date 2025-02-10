using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Components.Metrics;

internal sealed class VostokMetricsBuilder(IServiceCollection services) : IVostokMetricsBuilder
{
    public IServiceCollection Services { get; } = services;
}