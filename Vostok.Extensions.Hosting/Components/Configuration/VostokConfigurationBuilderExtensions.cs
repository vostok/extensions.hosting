using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Vostok.Configuration.Sources;

namespace Vostok.Extensions.Hosting.Components.Configuration;

[PublicAPI]
public static class VostokConfigurationBuilderExtensions
{
    public static IVostokConfigurationBuilder SetupSourceFor<T>(
        this IVostokConfigurationBuilder builder,
        params string[]? scope)
    {
        builder.Services.AddSingleton<ConfigurationProviderSetup>((provider, source) =>
        {
            source = scope is null || scope.Length == 0
                ? source
                : source.ScopeTo(scope);

            provider.SetupSourceFor<T>(source);
        });

        return builder;
    }
}