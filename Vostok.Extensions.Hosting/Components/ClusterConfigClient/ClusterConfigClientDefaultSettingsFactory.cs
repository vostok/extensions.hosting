using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Clusterclient.Tracing;
using Vostok.ClusterConfig.Client;
using Vostok.Configuration.Abstractions.Merging;
using Vostok.Configuration.Sources.Json;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.Tracing.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ClusterConfigClient;

internal sealed class ClusterConfigClientDefaultSettingsFactory(
    IEnumerable<IConfigureOptions<ClusterConfigClientSettings>> setups,
    IEnumerable<IPostConfigureOptions<ClusterConfigClientSettings>> postConfigures,
    IEnumerable<IValidateOptions<ClusterConfigClientSettings>> validations,
    IServiceProvider provider) : OptionsFactory<ClusterConfigClientSettings>(setups, postConfigures, validations)
{
    protected override ClusterConfigClientSettings CreateInstance(string _)
    {
        var log = provider.GetInitializationLog();
        var tracer = provider.GetRequiredService<ITracer>();

        return new ClusterConfigClientSettings
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
    }
}