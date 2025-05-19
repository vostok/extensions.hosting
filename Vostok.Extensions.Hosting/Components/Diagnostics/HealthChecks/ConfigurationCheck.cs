using Vostok.Configuration;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.Hosting.Components.Diagnostics.HealthChecks;

internal sealed class ConfigurationCheck(ConfigurationProvider configurationProvider) : IHealthCheck
{
    public Task<HealthCheckResult> CheckAsync(CancellationToken cancellationToken)
    {
        var reason = configurationProvider.GetHealthCheckResult();

        var result = new HealthCheckResult(reason.Error == null ? HealthStatus.Healthy : HealthStatus.Degraded, reason.Error);

        return Task.FromResult(result);
    }
}