using System.Net;

namespace Vostok.Extensions.Hosting.Components.Datacenters;

// todo: Rename
internal sealed class DatacentersOptions
{
    public required Func<IPAddress, string> DatacenterMapping { get; set; }
    public required Func<IReadOnlyCollection<string>> ActiveDatacentersProvider { get; set; }
}