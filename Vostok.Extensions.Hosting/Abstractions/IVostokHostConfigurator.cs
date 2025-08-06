using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Vostok.Extensions.Hosting.Abstractions;

public interface IVostokHostConfigurator
{
    public void ConfigureAfterBuild(WebApplication application) => throw new NotImplementedException();

    public void ConfigureAfterBuild(IHost host) => throw new NotImplementedException();
}