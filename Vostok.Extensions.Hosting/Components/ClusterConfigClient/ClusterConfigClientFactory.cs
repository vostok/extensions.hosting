using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Clusterclient.Tracing;
using Vostok.ClusterConfig.Client;
using Vostok.ClusterConfig.Client.Abstractions;
using Vostok.Configuration.Abstractions.Merging;
using Vostok.Configuration.Sources.Json;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.Logging.Abstractions;
using Vostok.Tracing.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ClusterConfigClient;

internal static class ClusterConfigClientFactory
{
    public static IClusterConfigClient Create(IServiceProvider provider)
    {
        var log = provider.GetRequiredService<ILog>();
        var tracer = provider.GetRequiredService<ITracer>();
        var options = provider.GetRequiredService<IOptions<ClusterConfigClientConfigurationOptions>>().Value;

        var settings = new ClusterConfigClientSettings
        {
            Log = log,
            AdditionalSetup = setup => setup.SetupDistributedTracing(tracer),
            MergeOptions = new SettingsMergeOptions
            {
                CustomMerge = (a, b) =>
                {
                    var ok = JsonConfigurationMerger.TryMerge(a, b, out var merged);
                    return (ok, merged);
                }
            }
        };

        options.ApplyCustomizations(settings);

        if (settings is { EnableLocalSettings: false, EnableClusterSettings: false })
        {
            log.LogDisabled("ClusterConfigClient", "explicit configuration");
            return new DisabledClusterConfigClient();
        }

        return new ClusterConfig.Client.ClusterConfigClient(settings);
    }
}