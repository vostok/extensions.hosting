using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Components.Hercules;
using Vostok.Hercules.Client;
using Vostok.Hercules.Client.Abstractions;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class VostokBuilderHerculesSinkExtensions
{
    public static IVostokBuilder WithHerculesSink(
        this IVostokBuilder builder,
        Action<IHerculesSinkBuilder>? configure = null)
    {
        builder.ConfigureServices(services =>
        {
            services.AddHerculesSink();

            var sinkBuilder = new HerculesSinkBuilder(services);
            configure?.Invoke(sinkBuilder);
        });

        return builder;
    }

    public static IVostokBuilder DisableHerculesSink(this IVostokBuilder builder)
    {
        builder.ConfigureServices(services =>
            services.Replace(ServiceDescriptor.Singleton<IHerculesSink>(new DevNullHerculesSink())));

        return builder;
    }

    private static void AddHerculesSink(this IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IOptionsFactory<HerculesSinkSettings>, HerculesSinkDefaultSettingsFactory>());
        services.TryAddSingleton(HerculesSinkFactory.Create);
        services.AddHostedService<HerculesSinkMetricsHostedService>(); // todo: Move to Kontur extensions?
    }
}