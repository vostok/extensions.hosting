using Microsoft.Extensions.Hosting;

using Vostok.Extensions.Hosting.HostedServices;

namespace Vostok.Extensions.Hosting.Helpers;

internal sealed class VostokHostLifetime(IHostLifetime baseHostLifetime, VostokStartupService startupService)
    : IHostLifetime
{
    public async Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        await baseHostLifetime.WaitForStartAsync(cancellationToken);
        await startupService.WarmupVostokAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken) =>
        baseHostLifetime.StopAsync(cancellationToken);
}