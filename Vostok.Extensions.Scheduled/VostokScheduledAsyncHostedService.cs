using JetBrains.Annotations;

using Microsoft.Extensions.Hosting;

using Vostok.Applications.Scheduled;

namespace Vostok.Extensions.Scheduled;

[PublicAPI]
public abstract class VostokScheduledAsyncHostedService(IServiceProvider provider) : IHostedService, IAsyncDisposable
{
    private IScheduledActionsRunner? _runner;
    private Task? _runnerTask;
    private CancellationTokenSource? _cts;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _runner = await ScheduledApplicationHelper.InitializeAsync(provider, SetupAsync);
        _runnerTask = _runner.RunAsync(_cts.Token);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_cts != null)
        {
            await _cts.CancelAsync();
            try
            {
                await _runnerTask!.WaitAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // pass
            }
        }
    }

    public virtual Task DoDisposeAsync() => Task.CompletedTask;

    public async ValueTask DisposeAsync()
    {
        if (_runner != null)
        {
            (_runner as IDisposable)?.Dispose();
        }
        await DoDisposeAsync();
    }

    protected abstract Task SetupAsync(IScheduledActionsBuilder builder, IServiceProvider provider);
}