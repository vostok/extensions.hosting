using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Vostok.Extensions.Hosting.Components.Datacenters;

[PublicAPI]
public static class VostokBuilderDatacentersExtensions
{
    public static IVostokBuilder WithDatacenters(
        this IVostokBuilder vostok,
        Action<DatacentersOptions>? configure = null)
    {
        if (!vostok.CheckAndMarkComponentAdded("Datacenters"))
            vostok.Services.AddDatacenters();

        if (configure is not null)
            vostok.Services.Configure(configure);

        return vostok;
    }

    private static void AddDatacenters(this IServiceCollection services) =>
        services.TryAddSingleton(DatacentersFactory.Create);
}