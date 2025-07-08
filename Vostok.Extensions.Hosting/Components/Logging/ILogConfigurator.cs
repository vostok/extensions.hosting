using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Components.Logging;

[PublicAPI]
public interface ILogConfigurator
{
    IServiceCollection Services { get; }
    string Name { get; }
}