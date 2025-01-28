using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting;

public interface IVostokContextBuilder
{
    IServiceCollection Services { get; }
}