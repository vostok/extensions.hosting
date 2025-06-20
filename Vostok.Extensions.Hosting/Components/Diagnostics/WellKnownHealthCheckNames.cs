using JetBrains.Annotations;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

/// <summary>
/// Built-in hosting diagnostic health checks names.
/// </summary>
[PublicAPI]
public static class WellKnownHealthCheckNames
{
    public const string DatacenterWhitelist = "Datacenter whitelist";
    public const string ThreadPoolStarvation = "Thread pool";
    public const string ZooKeeperConnection = "ZooKeeper connection";
    public const string DnsResolution = "DNS resolution";
    public const string Configuration = "Configuration";

    // Tags names
    public const string VostokHealthCheckTag = "VostokHealthCheck";
    public const string LegacyVostokHealthCheckTag = "LegacyVostokHealthCheck";
}