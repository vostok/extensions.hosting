using Vostok.Clusterclient.Core;
using Vostok.Clusterclient.Core.Model;
using Vostok.Clusterclient.Core.Topology;
using Vostok.Clusterclient.Transport;
using Vostok.Extensions.AspNetCore.Middlewares.PingApi;
using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.AspNetCore.Helpers;

internal static class MiddlewaresWarmup
{
    public static async Task WarmupPingApi(Uri warmupUrl)
    {
        var client = new ClusterClient(
            new SilentLog(),
            configuration =>
            {
                configuration.ClusterProvider = new FixedClusterProvider(warmupUrl);
                configuration.SetupUniversalTransport();
            });

        await client.SendAsync(Request.Get(PingApiMiddleware.PingPath), TimeSpan.FromSeconds(20));
    }
}