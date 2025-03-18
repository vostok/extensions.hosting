using Microsoft.Extensions.Options;

using Vostok.Clusterclient.Tracing;
using Vostok.ClusterConfig.Client;
using Vostok.Configuration.Abstractions.Merging;
using Vostok.Configuration.Sources.Json;
using Vostok.Logging.Abstractions;
using Vostok.Tracing.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ClusterConfigClient;

internal sealed class ClusterConfigClientDefaultConfiguration(ILog log, ITracer tracer)
    : IConfigureOptions<ClusterConfigClientSettings>
{
    public void Configure(ClusterConfigClientSettings settings)
    {
        settings.Log = log;

        settings.AdditionalSetup = setup => setup.SetupDistributedTracing(tracer);

        settings.MergeOptions = new SettingsMergeOptions
        {
            CustomMerge = (a, b) =>
            {
                var ok = JsonConfigurationMerger.TryMerge(a, b, out var merged);
                return (ok, merged);
            }
        };
    }
}