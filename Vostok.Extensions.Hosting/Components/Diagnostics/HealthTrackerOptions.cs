using JetBrains.Annotations;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

[PublicAPI]
public class HealthTrackerOptions
{
    public TimeSpan ChecksPeriod { get; set; } = TimeSpan.FromSeconds(30);

    public bool AddDatacenterWhitelistCheck { get; set; } = true;

    public bool AddThreadPoolStarvationCheck { get; set; } = true;

    public bool AddZooKeeperConnectionCheck { get; set; } = true;

    public bool AddDnsResolutionCheck { get; set; } = true;
}