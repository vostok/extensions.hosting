using JetBrains.Annotations;

namespace Vostok.Extensions.Hosting.Components.ApplicationReplicationInfo;

[PublicAPI]
public sealed class ApplicationReplicationOptions
{
    public int InstanceIndex { get; set; }
    public int InstancesCount { get; set; }
}