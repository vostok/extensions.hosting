using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

internal static class HealthTrackerFactory
{
    public static HealthTracker Create(IServiceProvider serviceProvider)
    {
        var log = serviceProvider.GetRequiredService<ILog>();
        var options = serviceProvider.GetRequiredService<IOptions<HealthTrackerOptions>>().Value;
        var healthTracker = new HealthTracker(options.ChecksPeriod, log);

        // if (options.AddDatacenterWhitelistCheck)
        //     healthTracker.RegisterCheck(WellKnownHealthCheckNames.DatacenterWhitelist, new DatacenterWhitelistCheck(context.Datacenters));
        //
        // if (options.AddThreadPoolStarvationCheck)
        //     healthTracker.RegisterCheck(WellKnownHealthCheckNames.ThreadPoolStarvation, new ThreadPoolStarvationCheck());
        //
        // var zooKeeperClient = serviceProvider.GetRequiredService<IZooKeeperClient>();
        // if (options.AddZooKeeperConnectionCheck && zooKeeperClient is ZooKeeper.Client.ZooKeeperClient realClient)
        //     healthTracker.RegisterCheck(WellKnownHealthCheckNames.ZooKeeperConnection, new ZooKeeperConnectionCheck(realClient));
        //
        // if (options.AddDnsResolutionCheck && RuntimeDetector.IsDotNet50AndNewer)
        //     healthTracker.RegisterCheck(WellKnownHealthCheckNames.DnsResolution, context.RegisterDisposable(new DnsResolutionCheck()));
        //
        // if (options.AddConfigurationCheck)
        // {
        //     healthTracker.RegisterCheck(WellKnownHealthCheckNames.Configuration, context.RegisterDisposable(new ConfigurationCheck(context.ConfigurationProvider)));
        //     healthTracker.RegisterCheck(WellKnownHealthCheckNames.SecretConfiguration, context.RegisterDisposable(new ConfigurationCheck(context.SecretConfigurationProvider)));
        // }

        return healthTracker;
    }
}