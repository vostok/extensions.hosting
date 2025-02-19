using Microsoft.Extensions.Hosting;

namespace Vostok.Extensions.Hosting.Components.Logging.LevelCounter;

internal sealed class LogLevelMetricsHostedService(LogLevelMetrics metrics) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        metrics.StartCollectingMetrics();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}