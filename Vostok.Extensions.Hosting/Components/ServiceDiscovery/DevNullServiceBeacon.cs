using Vostok.ServiceDiscovery.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

internal sealed class DevNullServiceBeacon(IReplicaInfo replicaInfo) : IServiceBeacon
{
    public IReplicaInfo ReplicaInfo { get; } = replicaInfo;

    public void Start()
    {
    }

    public void Stop()
    {
    }
}