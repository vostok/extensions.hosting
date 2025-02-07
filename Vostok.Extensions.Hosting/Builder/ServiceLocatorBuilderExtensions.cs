using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.Components.ServiceDiscovery;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class ServiceLocatorBuilderExtensions
{
    public static IVostokBuilder WithServiceLocator(
        this IVostokBuilder builder,
        Action<ServiceLocatorConfigurationOptions> configure)
    {
        builder.WithServiceLocator((settings, _) => configure(settings));

        return builder;
    }

    public static IVostokBuilder WithServiceLocator(
        this IVostokBuilder builder,
        Action<ServiceLocatorConfigurationOptions, IServiceProvider>? configure = null)
    {
        builder.Services.AddServiceLocator();

        if (configure is not null)
            builder.Services.AddOptions<ServiceLocatorConfigurationOptions>().Configure(configure);

        return builder;
    }

    private static void AddServiceLocator(this IServiceCollection services) =>
        services.TryAddSingleton(ServiceLocatorFactory.Create);
}