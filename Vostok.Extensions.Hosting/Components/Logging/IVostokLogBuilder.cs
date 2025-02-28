using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Components.Logging;

public interface IVostokLogBuilder
{
    IServiceCollection Services { get; }
}