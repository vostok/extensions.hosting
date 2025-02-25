using Vostok.Configuration.Primitives;
using Vostok.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.Hosting.Components.Diagnostics.InfoProviders;

internal class ApplicationInfoProvider(
    IVostokApplicationIdentity identity,
    IVostokApplicationLimits limits,
    Func<IVostokApplicationReplicationInfo> replication) : IDiagnosticInfoProvider
{
    public object Query() => new
    {
        Identity = new
        {
            identity.Project,
            identity.Subproject,
            identity.Application,
            identity.Environment,
            identity.Instance
        },
        Replication = replication(),
        Limits = new
        {
            Cpu = limits.CpuUnits.HasValue ? $"{limits.CpuUnits.Value:F2} core(s)" : "<unlimited>",
            Memory = limits.MemoryBytes.HasValue ? limits.MemoryBytes.Value.Bytes().ToString() : "<unlimited>"
        }
    };
}