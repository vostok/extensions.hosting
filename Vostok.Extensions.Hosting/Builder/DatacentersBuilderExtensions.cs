using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.Components.Datacenters;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class DatacentersBuilderExtensions
{
    public static IVostokBuilder WithDatacenters(
        this IVostokBuilder builder,
        Action<DatacentersConfigurationOptions> configure)
    {
        builder.WithDatacenters((options, _) => configure(options));

        return builder;
    }

    public static IVostokBuilder WithDatacenters(
        this IVostokBuilder builder,
        Action<DatacentersConfigurationOptions, IServiceProvider>? configure = null)
    {
        builder.Services.AddDatacenters();

        if (configure is not null)
            builder.Services.AddOptions<DatacentersConfigurationOptions>().Configure(configure);

        return builder;
    }

    private static void AddDatacenters(this IServiceCollection services) =>
        services.TryAddSingleton(DatacentersFactory.Create);
}