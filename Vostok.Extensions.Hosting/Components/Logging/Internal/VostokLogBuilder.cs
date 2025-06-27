using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Components.Logging.Internal;

internal sealed class VostokLogBuilder(IServiceCollection services) : IVostokLogBuilder
{
    public IServiceCollection Services { get; } = services;
}