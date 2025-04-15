using Vostok.Hosting.Abstractions.Diagnostics;
using Vostok.ZooKeeper.Client.Abstractions;
using Vostok.ZooKeeper.Client.Abstractions.Model;

namespace Vostok.Extensions.Hosting.Components.Diagnostics.HealthChecks;

internal sealed class ZooKeeperConnectionCheck(IZooKeeperClient client) : IHealthCheck
{
    private volatile bool _warmedUp;

    public async Task<HealthCheckResult> CheckAsync(CancellationToken cancellationToken)
    {
        await WarmupIfNeededAsync().ConfigureAwait(false);

        var currentConnectionState = client.ConnectionState;

        switch (currentConnectionState)
        {
            case ConnectionState.Connected:
            case ConnectionState.ConnectedReadonly:
                return HealthCheckResult.Healthy();

            default:
                return HealthCheckResult.Degraded($"ZooKeeper client is not connected. " +
                                                  $"Current connection state = '{currentConnectionState}'.");
        }
    }

    private async Task WarmupIfNeededAsync()
    {
        if (_warmedUp)
            return;

        await client.ExistsAsync("/").ConfigureAwait(false);

        _warmedUp = true;
    }
}