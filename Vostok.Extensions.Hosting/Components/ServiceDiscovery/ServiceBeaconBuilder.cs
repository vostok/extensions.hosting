using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

internal sealed class ServiceBeaconBuilder(IServiceCollection services) : IServiceBeaconBuilder
{
    public IServiceCollection Services { get; } = services;
}