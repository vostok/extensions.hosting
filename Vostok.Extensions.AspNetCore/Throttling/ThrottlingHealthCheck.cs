using Microsoft.Extensions.Diagnostics.HealthChecks;

using Vostok.Throttling;

namespace Vostok.Extensions.AspNetCore.Throttling;

internal sealed class ThrottlingHealthCheck(ThrottlingProvider throttlingProvider) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken _ = default)
    {
        var info = throttlingProvider.CurrentInfo;

        if (info.Enabled && info.QueueSize > 0)
            return Task.FromResult(HealthCheckResult.Degraded($"There's a throttling queue of size {info.QueueSize}."));

        return Task.FromResult(HealthCheckResult.Healthy());
    }
}