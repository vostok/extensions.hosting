using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Components.Logging.Builder;

internal sealed class VostokLogBuilder(IServiceCollection services) : IVostokLogBuilder
{
    public IServiceCollection Services { get; } = services;
}