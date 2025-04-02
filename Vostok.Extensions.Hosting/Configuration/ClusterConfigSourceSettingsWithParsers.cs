using Vostok.ClusterConfig.Client.Abstractions;
using Vostok.Configuration.Sources.ClusterConfig;
using Vostok.Configuration.Sources.Json;
using Vostok.Configuration.Sources.Xml;
using Vostok.Configuration.Sources.Yaml;

namespace Vostok.Extensions.Hosting.Configuration;

internal sealed class ClusterConfigSourceSettingsWithParsers : ClusterConfigSourceSettings
{
    public ClusterConfigSourceSettingsWithParsers(IClusterConfigClient client, string prefix) : base(client, prefix)
    {
        ConditionalValuesParsers =
        [
            (XmlConfigurationParser.Parse, node => node.Name != null && node.Name.EndsWith(".xml")),
            (JsonConfigurationParser.Parse, node => node.Name != null && node.Name.EndsWith(".json")),
            (YamlConfigurationParser.Parse, node => node.Name != null && (node.Name.EndsWith(".yaml") || node.Name.EndsWith("yml")))
        ];
    }
}