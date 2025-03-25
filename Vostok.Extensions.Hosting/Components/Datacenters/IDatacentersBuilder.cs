using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Components.Datacenters;

[PublicAPI]
public interface IDatacentersBuilder
{
    IServiceCollection Services { get; }
}