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
    public static IVostokBuilder WithLogging(this IVostokBuilder vostok, Action<IVostokLogBuilder> configure)
    {
        if (!vostok.CheckComponentAlreadyAdded("Logging"))
            vostok.Services.AddLogging();

        var vostokLogBuilder = new VostokLogBuilder(vostok.Services);
        configure(vostokLogBuilder);

        return vostok;
    }

    private static void AddLogging(this IServiceCollection services)
    {
        // note: Register ILog separately to let DI infrastructure dispose it if needed.
        services.TryAddKeyedSingleton<ILog>(
            VostokLogProviderFactory.InternalLogKey,
            (provider, _) => VostokLogProviderFactory.CreateInternalVostokLog(provider));

        var descriptor =
            ServiceDescriptor.Singleton<ILoggerProvider, VostokLoggerProvider>(VostokLogProviderFactory.Create);
        services.TryAddEnumerable(descriptor);
    }
}