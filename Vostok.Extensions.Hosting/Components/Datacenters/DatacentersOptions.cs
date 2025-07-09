using System.Diagnostics.CodeAnalysis;
using System.Net;

using JetBrains.Annotations;

using Vostok.Datacenters;

namespace Vostok.Extensions.Hosting.Components.Datacenters;

// note(ponomaryovigor): Copied from native DatacentersSettings with providers inlining and constructor removed.
// Need for use in Options Pattern without specifying a lot of boilerplate (options factories, helper options, etc).

/// <summary>
/// Represents <see cref="Datacenters"/> options.
/// </summary>
[PublicAPI]
public sealed class DatacentersOptions
{
    [DisallowNull]
    public required Func<IPAddress, string>? DatacenterMapping { get; set; }

    [DisallowNull]
    public required Func<IReadOnlyCollection<string>>? ActiveDatacentersProvider { get; set; }

    public TimeSpan DnsCacheTtl { get; set; } = TimeSpan.FromMinutes(30);
    public TimeSpan DnsResolveTimeout { get; set; } = TimeSpan.FromSeconds(1);

    public string? LocalDatacenter { get; set; }
    public string? LocalHostname { get; set; }
    public ResolveHostname? ResolveHostname { get; set; }
}