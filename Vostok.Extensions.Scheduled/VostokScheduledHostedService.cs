using JetBrains.Annotations;

using Microsoft.Extensions.Hosting;

using Vostok.Applications.Scheduled;

namespace Vostok.Extensions.Scheduled;

[PublicAPI]
public abstract class VostokScheduledHostedService(IScheduledActionsBuilder builder) : BackgroundService
{
    private IScheduledActionsRunner _runner = null!;

    protected sealed override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (builder is not ScheduledActionsBuilder actualBuilder)
            throw new InvalidOperationException(
                $"Scheduled actions builder has unsuitable implementation type. Should be {typeof(ScheduledActionsBuilder)}, but found {builder.GetType().FullName}");

        _runner = actualBuilder.BuildRunner();

        return _runner.RunAsync(stoppingToken);
    }

    public override void Dispose()
    {
        base.Dispose();

        (_runner as IDisposable)?.Dispose();
    }
}