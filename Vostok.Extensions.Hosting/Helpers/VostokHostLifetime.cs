using Microsoft.Extensions.Hosting;

using Vostok.Extensions.Hosting.HostedServices;

namespace Vostok.Extensions.Hosting.Helpers;

internal sealed class VostokHostLifetime(IHostLifetime baseHostLifetime, VostokWarmup warmup)
    : IHostLifetime
{
    public async Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        await baseHostLifetime.WaitForStartAsync(cancellationToken);
        await warmup.WarmupVostokAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken) =>
        baseHostLifetime.StopAsync(cancellationToken);
}