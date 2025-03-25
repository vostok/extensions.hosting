using System.Net;

namespace Vostok.Extensions.Hosting.Components.Datacenters;

// TODO: Think about something better
public sealed class DatacentersProvidersOptions
{
    public required Func<IPAddress, string> DatacenterMapping { get; set; }
    public required Func<IReadOnlyCollection<string>> ActiveDatacentersProvider { get; set; }
}