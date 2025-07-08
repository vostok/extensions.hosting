using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Components.Logging.Internal;

internal sealed class LogConfigurator(IServiceCollection services, string name) : ILogConfigurator
{
    public IServiceCollection Services { get; } = services;
    public string Name { get; } = name;
}