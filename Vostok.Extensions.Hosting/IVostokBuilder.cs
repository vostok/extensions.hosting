using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting;

public interface IVostokBuilder
{
    IServiceCollection Services { get; }
}