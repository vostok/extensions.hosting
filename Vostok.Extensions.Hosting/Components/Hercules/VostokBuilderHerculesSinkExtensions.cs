using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Hercules.Client;
using Vostok.Hercules.Client.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Hercules;

[PublicAPI]
public static class VostokBuilderHerculesSinkExtensions
{
    /// <summary>
    /// Registers and configures services for <see cref="IHerculesSink"/>.
    /// </summary>
    /// <param name="vostok">The <see cref="IVostokBuilder"/>.</param>
    /// <param name="configure">Delegate to configure <see cref="IHerculesSinkBuilder"/>.</param>
    /// <returns><see cref="IVostokBuilder"/> for chain method calls.</returns>
    /// <remarks>Calling this method multiple times on the same <see cref="IVostokBuilder"/> instance will result in one <see cref="IHerculesSink"/> configured.</remarks>
    public static IVostokBuilder WithHerculesSink(
        this IVostokBuilder vostok,
        Action<IHerculesSinkBuilder>? configure = null)
    {
        if (!vostok.CheckComponentAlreadyAdded("HerculesSink"))
            vostok.Services.AddHerculesSink();

        var sinkBuilder = new HerculesSinkBuilder(vostok.Services);
        configure?.Invoke(sinkBuilder);

        return vostok;
    }

    public static IVostokBuilder DisableHerculesSink(this IVostokBuilder vostok)
    {
        vostok.Services.Replace(ServiceDescriptor.Singleton(HerculesSinkFactory.CreateDisabled));
        return vostok;
    }

    private static void AddHerculesSink(this IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IOptionsFactory<HerculesSinkSettings>, HerculesSinkDefaultSettingsFactory>());
        services.TryAddSingleton(HerculesSinkFactory.Create);
    }
}