using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Components.ZooKeeperClient;

internal sealed class ZooKeeperClientBuilder(IServiceCollection services) : IZooKeeperClientBuilder
{
    public IServiceCollection Services { get; } = services;
}