using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ApplicationReplicationInfo;

internal sealed class VostokApplicationReplicationInfo : IVostokApplicationReplicationInfo
{
    public int InstanceIndex { get; }
    public int InstancesCount { get; }

    public VostokApplicationReplicationInfo(int instanceIndex, int instancesCount)
    {
        if (instanceIndex < 0 || instanceIndex >= instancesCount)
            throw new ArgumentOutOfRangeException($"Instance index ({instanceIndex}) not in range [0, {instancesCount - 1}].");

        InstanceIndex = instanceIndex;
        InstancesCount = instancesCount;
    }

    public override string ToString() => $"InstanceIndex = {InstanceIndex}; InstancesCount = {InstancesCount}";
}