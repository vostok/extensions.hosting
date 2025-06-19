using Microsoft.Extensions.Diagnostics.HealthChecks;
using Vostok.Commons.Threading;

namespace Vostok.Extensions.Hosting.Components.Diagnostics.HealthChecks;

internal sealed class ThreadPoolStarvationCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken _ = default)
    {
        var state = ThreadPoolUtility.GetPoolState();

        if (state.UsedWorkerThreads >= state.MinWorkerThreads)
        {
            var healthCheckResult = HealthCheckResult.Degraded(
                $"Worker threads in thread pool are exhausted: " +
                $"{state.UsedWorkerThreads}/{state.MinWorkerThreads} (used/min).");

            return Task.FromResult(healthCheckResult);
        }

        if (state.UsedIocpThreads >= state.MinIocpThreads)
        {
            var healthCheckResult = HealthCheckResult.Degraded(
                $"IOCP threads in thread pool are exhausted: " +
                $"{state.UsedIocpThreads}/{state.MinIocpThreads} (used/min).");

            return Task.FromResult(healthCheckResult);
        }

        return Task.FromResult(HealthCheckResult.Healthy());
    }
}