using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

[PublicAPI]
public interface IZooKeeperClientBuilder
{
    IServiceCollection Services { get; }
}