using Microsoft.Extensions.Diagnostics.HealthChecks;

using Vostok.Configuration;

namespace Vostok.Extensions.Hosting.Components.Diagnostics.HealthChecks;

internal sealed class ConfigurationCheck(ConfigurationProvider configurationProvider) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken _ = default)
    {
        var reason = configurationProvider.GetHealthCheckResult();

        var result = new HealthCheckResult(reason.Error == null ? HealthStatus.Healthy : HealthStatus.Degraded, reason.Error);

        return Task.FromResult(result);
    }
}