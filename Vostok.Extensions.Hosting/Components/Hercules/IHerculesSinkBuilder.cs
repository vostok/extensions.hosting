using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Components.Hercules;

public interface IHerculesSinkBuilder
{
    IServiceCollection Services { get; }
}