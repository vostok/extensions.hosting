using Vostok.Clusterclient.Core.Topology;

namespace Vostok.Extensions.Hosting.Components.Hercules;

internal sealed class HerculesSinkMiscellaneousOptions
{
    public Func<IServiceProvider, IClusterProvider?>? ClusterProvider { get; set; }
}