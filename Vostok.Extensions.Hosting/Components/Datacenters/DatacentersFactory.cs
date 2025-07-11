using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Datacenters;
using Vostok.Extensions.Hosting.Helpers;

namespace Vostok.Extensions.Hosting.Components.Datacenters;

internal static class DatacentersFactory
{
    public static IDatacenters Create(IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<DatacentersOptions>>().Value;

        if (options.DatacenterMapping is null || options.ActiveDatacentersProvider is null)
        {
            provider.GetInitializationLog().LogDisabled("Datacenters", "Unconfigured providers");
            return new EmptyDatacenters();
        }

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