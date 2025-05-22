using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Datacenters;

namespace Vostok.Extensions.Hosting.Components.Datacenters;

internal static class DatacentersFactory
{
    public static IDatacenters Create(IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<DatacentersOptions>>().Value;

        var settings = new DatacentersSettings(options.DatacenterMapping, options.ActiveDatacentersProvider)
        {
            DnsCacheTtl = options.DnsCacheTtl,
            DnsResolveTimeout = options.DnsResolveTimeout,
            LocalDatacenter = options.LocalDatacenter,
            LocalHostname = options.LocalHostname,
            ResolveHostname = options.ResolveHostname
        };

        return new Vostok.Datacenters.Datacenters(settings);
    }
}