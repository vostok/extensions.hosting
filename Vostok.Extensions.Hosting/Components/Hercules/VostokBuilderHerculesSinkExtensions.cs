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
        vostok.Services.Replace(ServiceDescriptor.Singleton<IHerculesSink>(new DevNullHerculesSink()));
        return vostok;
    }

    private static void AddHerculesSink(this IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IOptionsFactory<HerculesSinkSettings>, HerculesSinkDefaultSettingsFactory>());
        services.TryAddSingleton(HerculesSinkFactory.Create);
        services.AddHostedService<HerculesSinkMetricsHostedService>(); // todo: Move to Kontur extensions?
    }
}