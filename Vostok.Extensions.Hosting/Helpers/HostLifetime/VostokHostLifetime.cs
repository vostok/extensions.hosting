using Microsoft.Extensions.Hosting;

namespace Vostok.Extensions.Hosting.Helpers.HostLifetime;

internal sealed class VostokHostLifetime(IHostLifetime baseHostLifetime, VostokWarmup warmup) : IHostLifetime
{
    public async Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        await baseHostLifetime.WaitForStartAsync(cancellationToken).ConfigureAwait(false);
        await warmup.WarmupVostokAsync().ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellationToken) =>
        baseHostLifetime.StopAsync(cancellationToken);
}