using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

using Vostok.Extensions.Hosting.Components.Logging.Internal;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Microsoft;

namespace Vostok.Extensions.Hosting.Components.Logging;

[PublicAPI]
public static class VostokBuilderLoggingExtensions
{
    /// <summary>
    /// Registers and configures services for Vostok logging components.
    /// </summary>
    /// <param name="vostok">The <see cref="IVostokBuilder"/>.</param>
    /// <param name="configure">Delegate to configure <see cref="IVostokLogBuilder"/>.</param>
    /// <returns><see cref="IVostokBuilder"/> for chain method calls.</returns>
    /// <remarks>Calling this method multiple times on the same <see cref="IVostokBuilder"/> instance will result in one logging components configured.</remarks>
    public static IVostokBuilder WithLogging(this IVostokBuilder vostok, Action<IVostokLogBuilder> configure)
    {
        if (!vostok.CheckAndMarkComponentAdded("Logging"))
            vostok.Services.AddLogging();

        var vostokLogBuilder = new VostokLogBuilder(vostok.Services);
        configure(vostokLogBuilder);

        return vostok;
    }

    private static void AddLogging(this IServiceCollection services)
    {
        // note: Register ILog separately to let DI infrastructure dispose it.
        services.TryAddKeyedSingleton<ILog>(
            VostokLogFactory.LogKey,
            (provider, _) => VostokLogFactory.CreateVostokLogImplementation(provider));

        var descriptor =
            ServiceDescriptor.Singleton<ILoggerProvider, VostokLoggerProvider>(VostokLogFactory.CreateLoggerProvider);
        services.TryAddEnumerable(descriptor);
    }
}