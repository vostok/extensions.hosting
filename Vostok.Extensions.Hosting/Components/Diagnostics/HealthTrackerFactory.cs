using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Configuration;
using Vostok.Configuration.Abstractions;
using Vostok.Datacenters;
using Vostok.Extensions.Hosting.Components.Diagnostics.HealthChecks;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.Hosting.Abstractions.Diagnostics;
using Vostok.Logging.Abstractions;
using Vostok.Metrics.System.Process;
using Vostok.ZooKeeper.Client.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

internal static class HealthTrackerFactory
{
    public static IHealthTracker Create(IServiceProvider provider)
    {
        var log = provider.GetRequiredService<ILog>();
        var options = provider.GetRequiredService<IOptions<HealthTrackerOptions>>().Value;
        var healthTracker = new HealthTracker(options.ChecksPeriod, log);

        var datacenters = provider.GetService<IDatacenters>();
        if (options.AddDatacenterWhitelistCheck && datacenters is not null)
        {
            var check = new DatacenterWhitelistCheck(datacenters);
            provider.RegisterDisposable(healthTracker.RegisterCheck(WellKnownHealthCheckNames.DatacenterWhitelist, check));
        }

        if (options.AddThreadPoolStarvationCheck)
        {
            var check = new ThreadPoolStarvationCheck();
            provider.RegisterDisposable(healthTracker.RegisterCheck(WellKnownHealthCheckNames.ThreadPoolStarvation, check));
        }

        var zooKeeperClient = provider.GetService<IZooKeeperClient>();
        if (options.AddZooKeeperConnectionCheck && zooKeeperClient is ZooKeeper.Client.ZooKeeperClient actualClient)
        {
            var check = new ZooKeeperConnectionCheck(actualClient);
            provider.RegisterDisposable(healthTracker.RegisterCheck(WellKnownHealthCheckNames.ZooKeeperConnection, check));
        }

        var processMonitor = provider.GetService<CurrentProcessMonitor>();
        if (options.AddDnsResolutionCheck && processMonitor is not null)
        {
            var check = new DnsResolutionCheck(processMonitor);
            provider.RegisterDisposable(healthTracker.RegisterCheck(WellKnownHealthCheckNames.DnsResolution, check));

            provider.RegisterDisposable(check);
        }

        var configurationProvider = provider.GetService<IConfigurationProvider>();
        if (options.AddConfigurationCheck && configurationProvider is ConfigurationProvider actualProvider)
        {
            var check = new ConfigurationCheck(actualProvider);
            provider.RegisterDisposable(healthTracker.RegisterCheck(WellKnownHealthCheckNames.Configuration, check));
        }

        return healthTracker;
    }
}