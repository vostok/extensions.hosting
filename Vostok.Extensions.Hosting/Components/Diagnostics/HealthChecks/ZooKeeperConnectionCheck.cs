using Microsoft.Extensions.Diagnostics.HealthChecks;

using Vostok.ZooKeeper.Client.Abstractions;
using Vostok.ZooKeeper.Client.Abstractions.Model;

namespace Vostok.Extensions.Hosting.Components.Diagnostics.HealthChecks;

internal sealed class ZooKeeperConnectionCheck(IZooKeeperClient client) : IHealthCheck
{
    private volatile bool _warmedUp;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken _ = default)
    {
        await WarmupIfNeededAsync().ConfigureAwait(false);

        var currentConnectionState = client.ConnectionState;

        return currentConnectionState switch
        {
            ConnectionState.Connected or ConnectionState.ConnectedReadonly => HealthCheckResult.Healthy(),
            _ => HealthCheckResult.Degraded($"ZooKeeper client is not connected. Current connection state = '{currentConnectionState}'.")
        };
    }

    private async Task WarmupIfNeededAsync()
    {
        if (_warmedUp)
            return;

        await client.ExistsAsync("/").ConfigureAwait(false);

        _warmedUp = true;
    }
}