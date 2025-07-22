using Microsoft.Extensions.DependencyInjection;

using Vostok.Configuration.Sources;

namespace Vostok.Extensions.Hosting.Components.Configuration;

internal sealed class VostokConfigurationBuilder(IServiceCollection services) : IVostokConfigurationBuilder
{
    public IServiceCollection Services { get; } = services;

    public IVostokConfigurationBuilder SetupSourceFor<T>(params string[]? scope)
    {
        Services.Configure<VostokConfigurationMiscellaneousOptions>(options =>
            options.Customizations.Add((provider, source) =>
            {
                source = scope is null || scope.Length == 0
                    ? source
                    : source.ScopeTo(scope);

                provider.SetupSourceFor<T>(source);
            }));

        return this;
    }
}