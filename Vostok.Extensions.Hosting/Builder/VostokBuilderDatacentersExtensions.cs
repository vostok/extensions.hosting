using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.Components.Datacenters;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class VostokBuilderDatacentersExtensions
{
    public static IVostokBuilder WithDatacenters(
        this IVostokBuilder builder,
        Action<DatacentersOptions> configure)
    {
        builder.WithDatacenters((options, _) => configure(options));

        return builder;
    }

    public static IVostokBuilder WithDatacenters(
        this IVostokBuilder builder,
        Action<DatacentersOptions, IServiceProvider>? configure = null)
    {
        builder.ConfigureServices(services =>
        {
            services.AddDatacenters();

            if (configure is not null)
                services.AddOptions<DatacentersOptions>().Configure(configure);
        });

        return builder;
    }

    private static void AddDatacenters(this IServiceCollection services) =>
        services.TryAddSingleton(DatacentersFactory.Create);
}