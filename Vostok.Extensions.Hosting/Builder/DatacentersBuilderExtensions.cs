using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Datacenters;
using Vostok.Extensions.Hosting.Components.Datacenters;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class DatacentersBuilderExtensions
{
    internal const string ActualDatacentersKey = "_ActualDatacenters";

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

    internal static void PrepareDatacentersSubstitution(this IVostokBuilder builder)
    {
        builder.Services.AddOptionsWithValidateOnStart<SubstitutableDatacenters.SubstitutionMarker>();
        builder.Services.TryAddSingleton<IValidateOptions<SubstitutableDatacenters.SubstitutionMarker>, DatacenterSubstitutionValidator>();
    }

    private static void AddDatacenters(this IServiceCollection services)
    {
        // Do not add services if custom IDatacenters already registered
        if (services.Any(descriptor => descriptor.ServiceType == typeof(IDatacenters) &&
                                       descriptor.ImplementationType != typeof(SubstitutableDatacenters)))
        {
            return;
        }

        services.TryAddSingleton<IDatacenters, SubstitutableDatacenters>();
        services.TryAddKeyedSingleton<IDatacenters>(
            ActualDatacentersKey,
            (provider, _) => DatacentersFactory.Create(provider));
    }
}