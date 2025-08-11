using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Vostok.Extensions.Hosting.Abstractions;

public interface IVostokHostConfigurator
{
    //todo: подумать куда перенести эти методы. При этом конфигуратор может реализовывать оба, но вызывать надо подходящий
    public void ConfigureAfterBuild(WebApplication application) => throw new NotImplementedException();

    public void ConfigureAfterBuild(IHost host) => throw new NotImplementedException();
}