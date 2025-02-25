using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Commons.Environment;
using Vostok.Extensions.Hosting.Components.Diagnostics.HealthChecks;
using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

internal static class HealthTrackerFactory
{
    public static HealthTracker Create(IServiceProvider serviceProvider)
    {
        var log = serviceProvider.GetRequiredService<ILog>();
        var options = serviceProvider.GetRequiredService<IOptions<HealthTrackerOptions>>().Value;
        var healthTracker = new HealthTracker(options.ChecksPeriod, log);

        if (options.AddDatacenterWhitelistCheck)
        {
            var check = serviceProvider.GetRequiredService<DatacenterWhitelistCheck>();
            healthTracker.RegisterCheck(WellKnownHealthCheckNames.DatacenterWhitelist, check);
        }

        if (options.AddThreadPoolStarvationCheck)
        {
            var check = serviceProvider.GetRequiredService<ThreadPoolStarvationCheck>();
            healthTracker.RegisterCheck(WellKnownHealthCheckNames.ThreadPoolStarvation, check);
        }

        if (options.AddZooKeeperConnectionCheck)
        {
            var check = serviceProvider.GetRequiredService<ZooKeeperConnectionCheck>();
            healthTracker.RegisterCheck(WellKnownHealthCheckNames.ZooKeeperConnection, check);
        }

        if (options.AddDnsResolutionCheck && RuntimeDetector.IsDotNet50AndNewer)
        {
            var check = serviceProvider.GetRequiredService<DnsResolutionCheck>();
            healthTracker.RegisterCheck(WellKnownHealthCheckNames.DnsResolution, check);
        }

        if (options.AddConfigurationCheck)
        {
            // todo (bogdanov.n) 19.02.2025: requires configuration provider
            // and a way to distinguish between secret and common sources

            // healthTracker.RegisterCheck(WellKnownHealthCheckNames.Configuration, context.RegisterDisposable(new ConfigurationCheck(context.ConfigurationProvider)));
            // healthTracker.RegisterCheck(WellKnownHealthCheckNames.SecretConfiguration, context.RegisterDisposable(new ConfigurationCheck(context.SecretConfigurationProvider)));
        }

        return healthTracker;
    }
}