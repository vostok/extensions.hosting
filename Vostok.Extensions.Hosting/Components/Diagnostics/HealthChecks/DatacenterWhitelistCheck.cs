using Vostok.Datacenters;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.Hosting.Components.Diagnostics.HealthChecks;

internal sealed class DatacenterWhitelistCheck(IDatacenters datacenters) : IHealthCheck
{
    public Task<HealthCheckResult> CheckAsync(CancellationToken cancellationToken)
    {
        var localDatacenter = datacenters.GetLocalDatacenter();
        if (localDatacenter == null)
            return Task.FromResult(HealthCheckResult.Healthy());

        var activeDatacenters = datacenters.GetActiveDatacenters();
        if (activeDatacenters.Contains(localDatacenter))
            return Task.FromResult(HealthCheckResult.Healthy());

        return Task.FromResult(HealthCheckResult.Failing(
            $"Local datacenter '{localDatacenter}' is not among active datacenters " +
            $"({string.Join(", ", activeDatacenters.Select(d => $"'{d}'"))})."));
    }
}