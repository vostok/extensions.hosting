using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Vostok.Extensions.Hosting.Components.Datacenters;

[PublicAPI]
public static class VostokBuilderDatacentersExtensions
{
    public static IVostokBuilder WithDatacenters(
        this IVostokBuilder vostok,
        Action<DatacentersOptions> configure) =>
        vostok.WithDatacenters((options, _) => configure(options));

    public static IVostokBuilder WithDatacenters(
        this IVostokBuilder vostok,
        Action<DatacentersOptions, IServiceProvider>? configure = null)
    {
        if (!vostok.CheckComponentAlreadyAdded("Datacenters"))
            vostok.Services.AddDatacenters();

        if (configure is not null)
            vostok.Services.AddOptions<DatacentersOptions>().Configure(configure);

        return vostok;
    }

    private static void AddDatacenters(this IServiceCollection services) =>
        services.TryAddSingleton(DatacentersFactory.Create);
}