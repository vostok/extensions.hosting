using Microsoft.Extensions.DependencyInjection;
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
    public static void RegisterBuiltInHealthChecks(this IServiceCollection services) =>
        services.AddOptions<HealthCheckServiceOptions>()
                .Configure<IServiceProvider>(ConfigureHealthChecks);

    private static void ConfigureHealthChecks(HealthCheckServiceOptions options, IServiceProvider provider)
    {
        var checks = options.Registrations;

        var vostokOptions = provider.GetRequiredService<IOptions<BuiltInHealthChecksOptions>>().Value;

        var datacenters = provider.GetService<IDatacenters>();
        if (vostokOptions.AddDatacenterWhitelistCheck && datacenters is not null)
        {
            var check = new DatacenterWhitelistCheck(datacenters);
            checks.Add(Get(check, DatacenterWhitelist));
        }

        if (vostokOptions.AddThreadPoolStarvationCheck)
        {
            var check = new ThreadPoolStarvationCheck();
            checks.Add(Get(check, ThreadPoolStarvation));
        }

        var zooKeeperClient = provider.GetService<IZooKeeperClient>();
        if (vostokOptions.AddZooKeeperConnectionCheck && zooKeeperClient is ZooKeeper.Client.ZooKeeperClient actualClient)
        {
            var check = new ZooKeeperConnectionCheck(actualClient);
            checks.Add(Get(check, ZooKeeperConnection));
        }

        var processMonitor = provider.GetService<CurrentProcessMonitor>();
        if (vostokOptions.AddDnsResolutionCheck && processMonitor is not null)
        {
            var check = new DnsResolutionCheck(processMonitor);
            checks.Add(Get(check, DnsResolution));

            provider.RegisterDisposable(check);
        }

        var configurationProvider = provider.GetService<IConfigurationProvider>();
        if (vostokOptions.AddConfigurationCheck && configurationProvider is ConfigurationProvider actualProvider)
        {
            var check = new ConfigurationCheck(actualProvider);
            checks.Add(Get(check, WellKnownHealthCheckNames.Configuration));
        }
    }

    private static HealthCheckRegistration Get(IHealthCheck check, string name) =>
        VostokHealthCheckHelpers.RegisterWithVostokTag(name, check);
}