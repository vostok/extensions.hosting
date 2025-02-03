using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Datacenters;
using Vostok.Extensions.Hosting.Components.Datacenters;
using Vostok.Logging.Abstractions;

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

    private static void AddDatacenters(this IServiceCollection services)
    {
        // Do not add services if custom IDatacenters already registered
        if (services.Any(descriptor => descriptor.ServiceType == typeof(IDatacenters) &&
                                       descriptor.ImplementationType != typeof(SubstitutableDatacenters)))
        {
            return;
        }

        services.TryAddSingleton<IDatacenters, SubstitutableDatacenters>();
        services.AddOptionsWithValidateOnStart<DatacenterSubstitutionMarker>();
        services.TryAddSingleton<IValidateOptions<DatacenterSubstitutionMarker>, DatacenterSubstitutionValidator>();

        services.TryAddKeyedSingleton<IDatacenters>(ActualDatacentersKey, (provider, _) => CreateDatacenters(provider));
    }

    private static IDatacenters CreateDatacenters(IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<DatacentersConfigurationOptions>>().Value;
        var log = provider.GetRequiredService<ILog>();

        if (options.DatacenterMapping is null)
        {
            log.LogDisabled("Datacenters", "unconfigured mapping");
            return new EmptyDatacenters();
        }

        if (options.ActiveDatacentersProvider is null)
        {
            log.LogDisabled("Datacenters", "unconfigured active datacenters provider");
            return new EmptyDatacenters();
        }

        var datacentersSettings = new DatacentersSettings(
            options.DatacenterMapping,
            options.ActiveDatacentersProvider);

        options.ApplyCustomizations(datacentersSettings);

        return new Datacenters.Datacenters(datacentersSettings);
    }
}