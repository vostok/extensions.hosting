using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.Components.Configuration;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class VostokBuilderConfigurationExtensions
{
    public static IVostokBuilder WithConfiguration(
        this IVostokBuilder builder,
        Action<IVostokConfigurationBuilder>? configure = null)
    {
        builder.ConfigureServices(services =>
        {
            services.AddVostokConfiguration();

            var configurationBuilder = new VostokConfigurationBuilder(services);
            configure?.Invoke(configurationBuilder);
        });

        return builder;
    }

    private static void AddVostokConfiguration(this IServiceCollection services)
    {
        services.TryAddSingleton(ConfigurationFactory.CreateSource);
        services.TryAddSingleton(ConfigurationFactory.CreateProvider);
    }
}