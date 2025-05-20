using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting;

public interface IVostokBuilder
{
    void ConfigureServices(Action<IServiceCollection> configure);
}