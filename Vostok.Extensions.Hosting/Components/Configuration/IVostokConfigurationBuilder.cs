using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Components.Configuration;

[PublicAPI]
public interface IVostokConfigurationBuilder
{
    IServiceCollection Services { get; }

    IVostokConfigurationBuilder SetupSourceFor<T>(params string[]? scope);
}