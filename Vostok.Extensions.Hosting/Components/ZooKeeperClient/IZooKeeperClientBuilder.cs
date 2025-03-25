using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Components.ZooKeeperClient;

[PublicAPI]
public interface IZooKeeperClientBuilder
{
    IServiceCollection Services { get; }
}