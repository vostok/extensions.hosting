using Microsoft.Extensions.Options;

using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ApplicationReplicationInfo;

internal sealed class ApplicationReplicationInfo(IOptionsMonitor<ApplicationReplicationOptions> options)
    : IVostokApplicationReplicationInfo
{
    public int InstanceIndex => options.CurrentValue.InstanceIndex;
    public int InstancesCount => options.CurrentValue.InstancesCount;

    public override string ToString() => $"InstanceIndex = {InstanceIndex}; InstancesCount = {InstancesCount}";
}