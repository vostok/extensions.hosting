using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Vostok.Extensions.Hosting.Components.Configuration;

[PublicAPI]
public static class VostokBuilderConfigurationExtensions
{
    /// <summary>
    /// Registers and configures services for Vostok configuration components.
    /// </summary>
    /// <param name="vostok">The <see cref="IVostokBuilder"/>.</param>
    /// <param name="configure">Delegate to configure <see cref="IVostokConfigurationBuilder"/>.</param>
    /// <returns><see cref="IVostokBuilder"/> for chain method calls.</returns>
    /// <remarks>Calling this method multiple times on the same <see cref="IVostokBuilder"/> instance will result in one configuration components configured.</remarks>
    public static IVostokBuilder WithVostokConfiguration(
        this IVostokBuilder vostok,
        Action<IVostokConfigurationBuilder>? configure = null)
    {
        if (!vostok.CheckAndMarkComponentAdded("VostokConfiguration"))
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