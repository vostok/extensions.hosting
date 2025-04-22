using Vostok.Commons.Threading;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.Hosting.Components.Diagnostics.HealthChecks;

internal sealed class ThreadPoolStarvationCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckAsync(CancellationToken cancellationToken)
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