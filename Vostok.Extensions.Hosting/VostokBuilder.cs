using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Clusterclient.Tracing;
using Vostok.ClusterConfig.Client;
using Vostok.ClusterConfig.Client.Abstractions;
using Vostok.Configuration.Abstractions.Merging;
using Vostok.Configuration.Sources.Json;
using Vostok.Datacenters;
using Vostok.Extensions.Hosting.Components.ApplicationIdentity;
using Vostok.Extensions.Hosting.Components.ApplicationLimits;
using Vostok.Extensions.Hosting.Components.ApplicationReplicationInfo;
using Vostok.Extensions.Hosting.Components.ClusterConfigClient;
using Vostok.Extensions.Hosting.Components.Datacenters;
using Vostok.Hosting.Abstractions;
using Vostok.Logging.Abstractions;
using Vostok.Tracing.Abstractions;
using Vostok.Tracing.Diagnostics;

namespace Vostok.Extensions.Hosting;

internal sealed class VostokBuilder : IVostokBuilder
{
    public IServiceCollection Services { get; }

    public VostokBuilder(IServiceCollection services)
    {
        Services = services;

        AddApplicationIdentity(services);
        AddApplicationReplicationInfo(services);
        AddApplicationLimits(services);
        AddClusterConfigClient(services);
        AddDatacenters(services);
        AddTracer(services);
        AddLog(services);

        services.TryAddSingleton<IVostokContext, VostokContext>();
    }

    private static void AddApplicationIdentity(IServiceCollection services)
    {
        services.AddOptionsWithValidateOnStart<ApplicationIdentityOptions>();
        services.TryAddSingleton<IValidateOptions<ApplicationIdentityOptions>, ApplicationIdentityOptionsValidator>();

        services.TryAddSingleton<IVostokApplicationIdentity, ApplicationIdentity>();
    }

    private static void AddApplicationReplicationInfo(IServiceCollection services) =>
        services.TryAddSingleton<Func<IVostokApplicationReplicationInfo>>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<ApplicationReplicationInfoConfigurationOptions>>().Value;

            return options.GetProvider();
        });

    private static void AddApplicationLimits(IServiceCollection services) =>
        services.TryAddSingleton<IVostokApplicationLimits, ApplicationLimits>();

    private static void AddClusterConfigClient(IServiceCollection services)
    {
        services.TryAddSingleton<IClusterConfigClient>(provider =>
        {
            var log = provider.GetRequiredService<ILog>();
            var tracer = provider.GetRequiredService<ITracer>();
            var options = provider.GetRequiredService<IOptions<ClusterConfigClientConfigurationOptions>>().Value;

            var settings = new ClusterConfigClientSettings
            {
                Log = log,
                AdditionalSetup = setup => setup.SetupDistributedTracing(tracer),
                MergeOptions = new SettingsMergeOptions
                {
                    CustomMerge = (a, b) =>
                    {
                        var ok = JsonConfigurationMerger.TryMerge(a, b, out var merged);
                        return (ok, merged);
                    }
                }
            };

            options.ApplyCustomizations(settings);
            
            if (settings is { EnableLocalSettings: false, EnableClusterSettings: false })
            {
                // todo
                // context.LogDisabled("ClusterConfigClient", "explicit configuration");
                return new DisabledClusterConfigClient();
            }

            return new ClusterConfigClient(settings);
        });
    }

    private static void AddDatacenters(IServiceCollection services)
    {
        services.TryAddSingleton<IDatacenters>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<DatacentersConfigurationOptions>>().Value;

            if (options.DatacenterMapping is null || options.ActiveDatacentersProvider is null)
                // todo: Logging
                return new EmptyDatacenters();

            var datacentersSettings = new DatacentersSettings(
                options.DatacenterMapping,
                options.ActiveDatacentersProvider);

            options.ApplyCustomizations(datacentersSettings);

            return new Datacenters.Datacenters(datacentersSettings);
        });
    }

    private static void AddTracer(IServiceCollection services)
    {
        var activitySourceTracer = new ActivitySourceTracer(new ActivitySourceTracerSettings());
        services.TryAddSingleton<ITracer>(activitySourceTracer);
    }

    private static void AddLog(IServiceCollection services)
        => services.TryAddSingleton<ILog>(new SilentLog());
}