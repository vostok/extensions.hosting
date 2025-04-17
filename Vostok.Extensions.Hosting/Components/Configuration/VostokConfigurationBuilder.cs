using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Components.Configuration;

internal sealed class VostokConfigurationBuilder(IServiceCollection services) : IVostokConfigurationBuilder
{
    public IServiceCollection Services { get; } = services;
}