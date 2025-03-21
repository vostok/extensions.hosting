using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Components.Hercules;
using Vostok.Hercules.Client;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class HerculesSinkBuilderExtensions
{
    public static IVostokBuilder WithHerculesSink(
        this IVostokBuilder builder,
        Action<IHerculesSinkBuilder>? configure = null)
    {
        builder.Services.AddHerculesSink();

        var sinkBuilder = new HerculesSinkBuilder(builder.Services);
        configure?.Invoke(sinkBuilder);

        return builder;
    }

    private static void AddHerculesSink(this IServiceCollection services)
    {
        services.TryAddSingleton<IOptionsFactory<HerculesSinkSettings>, HerculesSinkSettingsFactory>();
        services.TryAddSingleton<IConfigureOptions<HerculesSinkSettings>, HerculesSinkInitialConfiguration>();
        services.TryAddSingleton(HerculesSinkFactory.Create);
        services.AddHostedService<HerculesSinkMetricsHostedService>();
    }
}