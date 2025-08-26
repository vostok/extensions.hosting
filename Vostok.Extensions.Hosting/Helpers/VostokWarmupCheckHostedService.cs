using Microsoft.Extensions.Hosting;

namespace Vostok.Extensions.Hosting.Helpers;

/// <summary>
/// Helper service that check warmup early without starting.
/// </summary>
internal sealed class VostokWarmupCheckHostedService : IHostedService
{
    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    public VostokWarmupCheckHostedService(VostokWarmup vostokWarmup)
    {
        if (!vostokWarmup.IsWarmedUp)
            throw new Exception(
                $"Vostok components weren't warmed up. It could happened in case of replacement of `{nameof(IHostLifetime)}`. " +
                "If you need to replace it, do it before Vostok registration, " +
                $"or call `host.{nameof(HostWarmupExtensions.WarmupVostok)}()` right after host build`.");
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}