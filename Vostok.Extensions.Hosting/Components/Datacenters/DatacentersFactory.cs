using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Datacenters;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Datacenters;

internal static class DatacentersFactory
{
    public static IDatacenters Create(IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<DatacentersConfigurationOptions>>().Value;
        var log = provider.GetRequiredService<ILog>();

        if (options.DatacenterMapping is null)
        {
            log.LogDisabled("Datacenters", "unconfigured mapping");
            return new EmptyDatacenters();
        }

        if (options.ActiveDatacentersProvider is null)
        {
            log.LogDisabled("Datacenters", "unconfigured active datacenters provider");
            return new EmptyDatacenters();
        }

        var datacentersSettings = new DatacentersSettings(options.DatacenterMapping, options.ActiveDatacentersProvider);

        options.ApplyCustomizations(datacentersSettings);

        return new Vostok.Datacenters.Datacenters(datacentersSettings);
    }
}