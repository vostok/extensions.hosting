using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Vostok.Extensions.Hosting.Components.Configuration;

[PublicAPI]
public static class VostokBuilderConfigurationExtensions
{
    public static IVostokBuilder WithConfiguration(
        this IVostokBuilder vostok,
        Action<IVostokConfigurationBuilder>? configure = null)
    {
        if (!vostok.CheckComponentAlreadyAdded("Configuration"))
            vostok.Services.AddVostokConfiguration();

        var configurationBuilder = new VostokConfigurationBuilder(vostok.Services);
        configure?.Invoke(configurationBuilder);

        return vostok;
    }

    private static void AddVostokConfiguration(this IServiceCollection services)
    {
        services.TryAddSingleton(ConfigurationFactory.CreateSource);
        services.TryAddSingleton(ConfigurationFactory.CreateProvider);
    }
}