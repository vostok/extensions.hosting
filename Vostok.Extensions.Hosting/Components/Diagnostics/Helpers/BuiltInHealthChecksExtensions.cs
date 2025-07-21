using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

using Vostok.Configuration;
using Vostok.Configuration.Abstractions;
using Vostok.Datacenters;
using Vostok.Extensions.Hosting.Components.Diagnostics.HealthChecks;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.Metrics.System.Process;
using Vostok.ZooKeeper.Client.Abstractions;

using static Vostok.Extensions.Hosting.Components.Diagnostics.WellKnownHealthCheckNames;

namespace Vostok.Extensions.Hosting.Components.Diagnostics.Helpers;

internal static class BuiltInHealthChecksExtensions
{
    public static void RegisterBuiltInHealthChecks(this IServiceCollection services)
    {
        var configure =
            ServiceDescriptor.Singleton<IConfigureOptions<HealthCheckServiceOptions>, ConfigureBuiltInHealthChecks>();

        services.TryAddEnumerable(configure);
    }
}

file sealed class ConfigureBuiltInHealthChecks(IServiceProvider provider) : IConfigureOptions<HealthCheckServiceOptions>
{
    public void Configure(HealthCheckServiceOptions options)
    {
        var checks = options.Registrations;

        var vostokOptions = provider.GetRequiredService<IOptions<BuiltInHealthChecksOptions>>().Value;

        var datacenters = provider.GetService<IDatacenters>();
        if (vostokOptions.AddDatacenterWhitelistCheck && datacenters is not null)
        {
            var check = new DatacenterWhitelistCheck(datacenters);
            Add(check, DatacenterWhitelist);
        }

        if (vostokOptions.AddThreadPoolStarvationCheck)
        {
            var check = new ThreadPoolStarvationCheck();
            Add(check, ThreadPoolStarvation);
        }

        var zooKeeperClient = provider.GetService<IZooKeeperClient>();
        if (vostokOptions.AddZooKeeperConnectionCheck && zooKeeperClient is ZooKeeper.Client.ZooKeeperClient actualClient)
        {
            var check = new ZooKeeperConnectionCheck(actualClient);
            Add(check, ZooKeeperConnection);
        }

        var processMonitor = provider.GetService<CurrentProcessMonitor>();
        if (vostokOptions.AddDnsResolutionCheck && processMonitor is not null)
        {
            var check = new DnsResolutionCheck(processMonitor);
            Add(check, DnsResolution);

            provider.RegisterDisposable(check);
        }

        var configurationProvider = provider.GetService<IConfigurationProvider>();
        if (vostokOptions.AddConfigurationCheck && configurationProvider is ConfigurationProvider actualProvider)
        {
            var check = new ConfigurationCheck(actualProvider);
            Add(check, WellKnownHealthCheckNames.Configuration);
        }

        return;

        void Add(IHealthCheck check, string name) =>
            checks.Add(VostokHealthCheckHelpers.RegisterWithVostokTag(name, check));
    }
}