using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Components.Hercules;

[PublicAPI]
public interface IHerculesSinkBuilder
{
    IServiceCollection Services { get; }
}