using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Components.Datacenters;

internal sealed class DatacentersBuilder(IServiceCollection services) : IDatacentersBuilder
{
    public IServiceCollection Services { get; } = services;
}