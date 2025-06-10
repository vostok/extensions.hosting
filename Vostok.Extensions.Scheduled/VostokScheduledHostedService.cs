using JetBrains.Annotations;

using Microsoft.Extensions.Hosting;

using Vostok.Applications.Scheduled;

namespace Vostok.Extensions.Scheduled;

[PublicAPI]
public abstract class VostokScheduledHostedService(IServiceProvider provider) : IHostedService, IAsyncDisposable
{
    private IScheduledActionsRunner? _runner;
    private Task? _runnerTask;
    private CancellationTokenSource? _cts;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _runner = await ScheduledApplicationHelper.InitializeAsync(provider, Setup);
        _runnerTask = _runner.RunAsync(_cts.Token);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_cts != null)
        {
            await _cts.CancelAsync();
            try
            {
                if (_runnerTask != null)
                    await _runnerTask.WaitAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // pass
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            // ReSharper disable once MethodHasAsyncOverload
            _cts?.Cancel();

            if (_runnerTask != null)
                await _runnerTask.WaitAsync(TimeSpan.FromSeconds(10));
        }
        catch (Exception)
        {
            // pass
        }

        (_runner as IDisposable)?.Dispose();
        await DoDisposeAsync();
        // ReSharper disable once MethodHasAsyncOverload
        DoDispose();
    }

    protected virtual void DoDispose() { }

    protected virtual Task DoDisposeAsync() => Task.CompletedTask;

    protected abstract void Setup(IScheduledActionsBuilder builder, IServiceProvider provider);
}