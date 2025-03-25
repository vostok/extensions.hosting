using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Datacenters;

namespace Vostok.Extensions.Hosting.Components.Datacenters;

internal static class DatacentersFactory
{
    public static IDatacenters Create(IServiceProvider provider)
    {
        var settings = provider.GetRequiredService<IOptions<DatacentersSettings>>().Value;
        return new Vostok.Datacenters.Datacenters(settings);
    }
}