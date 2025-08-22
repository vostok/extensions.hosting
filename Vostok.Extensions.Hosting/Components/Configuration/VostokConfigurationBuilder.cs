using Microsoft.Extensions.DependencyInjection;

using Vostok.Configuration.Sources;

namespace Vostok.Extensions.Hosting.Components.Configuration;

internal sealed class VostokConfigurationBuilder(IServiceCollection services) : IVostokConfigurationBuilder
{
    public IServiceCollection Services { get; } = services;

    public IVostokConfigurationBuilder SetupSourceFor<TSettings>(params string[]? scopes)
    {
        Services.Configure<VostokConfigurationMiscellaneousOptions>(options =>
            options.Customizations.Add((provider, source) =>
            {
                source = scopes is null || scopes.Length == 0
                    ? source
                    : source.ScopeTo(scopes);

                provider.SetupSourceFor<TSettings>(source);
            }));

        return this;
    }
}