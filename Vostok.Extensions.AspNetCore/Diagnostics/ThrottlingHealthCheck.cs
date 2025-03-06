using Vostok.Hosting.Abstractions.Diagnostics;
using Vostok.Throttling;

namespace Vostok.Extensions.AspNetCore.Diagnostics;

public class ThrottlingHealthCheck(ThrottlingProvider throttlingProvider) : IHealthCheck
{
    public Task<HealthCheckResult> CheckAsync(CancellationToken cancellationToken)
    {
        var info = throttlingProvider.CurrentInfo;

        if (info.Enabled && info.QueueSize > 0)
            return Task.FromResult(HealthCheckResult.Degraded($"There's a throttling queue of size {info.QueueSize}."));

        return Task.FromResult(HealthCheckResult.Healthy());
    }
}