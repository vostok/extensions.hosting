using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Datacenters;
using Vostok.Extensions.Hosting.Components.Diagnostics.HealthChecks;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.Logging.Abstractions;
using Vostok.Metrics.System.Process;
using Vostok.ZooKeeper.Client.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

internal static class HealthTrackerFactory
{
    public static HealthTracker Create(IServiceProvider provider)
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
        if (options.AddZooKeeperConnectionCheck && zooKeeperClient is ZooKeeper.Client.ZooKeeperClient realClient)
        {
            var check = new ZooKeeperConnectionCheck(realClient);
            provider.RegisterDisposable(healthTracker.RegisterCheck(WellKnownHealthCheckNames.ZooKeeperConnection, check));
        }

        var processMonitor = provider.GetService<CurrentProcessMonitor>();
        if (options.AddDnsResolutionCheck && processMonitor is not null)
        {
            var check = new DnsResolutionCheck(processMonitor);
            provider.RegisterDisposable(healthTracker.RegisterCheck(WellKnownHealthCheckNames.DnsResolution, check));

            provider.RegisterDisposable(check);
        }

        // todo (bogdanov.n) 14.02.2025: requires `IConfigurationSource`
        // if (options.AddConfigurationCheck)
        // {
            // healthTracker.RegisterCheck(WellKnownHealthCheckNames.Configuration, context.RegisterDisposable(new ConfigurationCheck(context.ConfigurationProvider)));
            // healthTracker.RegisterCheck(WellKnownHealthCheckNames.SecretConfiguration, context.RegisterDisposable(new ConfigurationCheck(context.SecretConfigurationProvider)));
        // }

        return healthTracker;
    }
}