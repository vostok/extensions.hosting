using Microsoft.Extensions.Diagnostics.HealthChecks;

using Vostok.Datacenters;

namespace Vostok.Extensions.Hosting.Components.Diagnostics.HealthChecks;

internal sealed class DatacenterWhitelistCheck(IDatacenters datacenters) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken _ = default)
    {
        var localDatacenter = datacenters.GetLocalDatacenter();
        if (localDatacenter == null)
            return Task.FromResult(HealthCheckResult.Healthy());

        var activeDatacenters = datacenters.GetActiveDatacenters();
        if (activeDatacenters.Contains(localDatacenter))
            return Task.FromResult(HealthCheckResult.Healthy());

        return Task.FromResult(HealthCheckResult.Unhealthy(
            $"Local datacenter '{localDatacenter}' is not among active datacenters " +
            $"({string.Join(", ", activeDatacenters.Select(d => $"'{d}'"))})."));
    }
}