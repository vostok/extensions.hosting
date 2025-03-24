using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Datacenters;
using Vostok.Extensions.Hosting.Components.Datacenters;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class VostokBuilderDatacentersExtensions
{
    public static IVostokBuilder WithDatacenters(
        this IVostokBuilder builder,
        Action<IDatacentersBuilder> configure)
    {
        builder.Services.AddDatacenters();

        var datacentersBuilder = new DatacentersBuilder(builder.Services);
        configure(datacentersBuilder);

        return builder;
    }

    private static void AddDatacenters(this IServiceCollection services)
    {
        services.TryAddSingleton<IOptionsFactory<DatacentersSettings>, DatacentersSettingsFactory>();
        services.TryAddSingleton(DatacentersFactory.Create);
    }
}