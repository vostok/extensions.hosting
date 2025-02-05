using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.Components.Hercules;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class HerculesSinkBuilderExtensions
{
    public static IVostokBuilder WithHerculesSink(
        this IVostokBuilder builder,
        Action<HerculesSinkConfigurationOptions> configure)
    {
        builder.WithHerculesSink((settings, _) => configure(settings));

        return builder;
    }

    public static IVostokBuilder WithHerculesSink(
        this IVostokBuilder builder,
        Action<HerculesSinkConfigurationOptions, IServiceProvider>? configure = null)
    {
        builder.Services.AddHerculesSink();

        if (configure is not null)
            builder.Services.AddOptions<HerculesSinkConfigurationOptions>().Configure(configure);

        return builder;
    }

    private static void AddHerculesSink(this IServiceCollection services) =>
        services.TryAddSingleton(HerculesSinkFactory.Create);
}