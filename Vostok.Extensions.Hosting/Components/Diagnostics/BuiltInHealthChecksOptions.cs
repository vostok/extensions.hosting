using JetBrains.Annotations;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

[PublicAPI]
public sealed class BuiltInHealthChecksOptions
{
    public bool AddDatacenterWhitelistCheck { get; set; } = true;
    public bool AddThreadPoolStarvationCheck { get; set; } = true;
    public bool AddZooKeeperConnectionCheck { get; set; } = true;
    public bool AddDnsResolutionCheck { get; set; } = true;
    public bool AddConfigurationCheck { get; set; } = true;
}