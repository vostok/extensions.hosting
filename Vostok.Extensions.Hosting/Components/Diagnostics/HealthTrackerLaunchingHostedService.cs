using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

internal sealed class HealthTrackerLaunchingHostedService(
    IHealthTracker healthTracker,
    IHostApplicationLifetime lifetime,
    ILogger<HealthTrackerLaunchingHostedService> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (healthTracker is not HealthTracker actualTracker)
        {
            logger.LogWarning(
                "Provided IHealthTracker instance is of unknown type {HealthTrackerType}, unable to launch periodical health checks.",
                healthTracker.GetType().Name);

            return Task.CompletedTask;
        }

        actualTracker.LaunchPeriodicalChecks(lifetime.ApplicationStopping);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}