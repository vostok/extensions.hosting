using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Components.Hercules;

internal sealed class HerculesSinkBuilder(IServiceCollection services) : IHerculesSinkBuilder
{
    public IServiceCollection Services { get; } = services;
}