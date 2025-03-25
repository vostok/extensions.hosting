using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Vostok.Datacenters;

namespace Vostok.Extensions.Hosting.Components.Datacenters;

[PublicAPI]
public static class DatacentersBuilderExtensions
{
    public static IDatacentersBuilder Configure(this IDatacentersBuilder builder, Action<DatacentersSettings> configure)
    {
        builder.Configure((settings, _) => configure(settings));
        return builder;
    }

    public static IDatacentersBuilder Configure(
        this IDatacentersBuilder builder,
        Action<DatacentersSettings, IServiceProvider>? configure = null)
    {
        if (configure is not null)
            builder.Services.AddOptions<DatacentersSettings>().Configure(configure);
        return builder;
    }

    public static IDatacentersBuilder ConfigureProviders(
        this IDatacentersBuilder builder,
        Action<DatacentersProvidersOptions> configure)
    {
        builder.ConfigureProviders((options, _) => configure(options));
        return builder;
    }

    public static IDatacentersBuilder ConfigureProviders(
        this IDatacentersBuilder builder,
        Action<DatacentersProvidersOptions, IServiceProvider>? configure = null)
    {
        if (configure is not null)
            builder.Services.AddOptions<DatacentersProvidersOptions>().Configure(configure);
        return builder;
    }
}