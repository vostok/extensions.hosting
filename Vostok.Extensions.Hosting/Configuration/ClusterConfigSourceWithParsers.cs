using Vostok.ClusterConfig.Client.Abstractions;
using Vostok.Configuration.Sources.ClusterConfig;
using Vostok.Configuration.Sources.Json;
using Vostok.Configuration.Sources.Xml;
using Vostok.Configuration.Sources.Yaml;

namespace Vostok.Extensions.Hosting.Configuration;

internal sealed class ClusterConfigSourceWithParsers(IClusterConfigClient client, string prefix)
    : ClusterConfigSource(new ClusterConfigSourceSettings(client, prefix)
    {
        ConditionalValuesParsers = new List<(ValueNodeParser, ValueNodeCondition)>
        {
            (XmlConfigurationParser.Parse, node => node.Name != null && node.Name.EndsWith(".xml")),
            (JsonConfigurationParser.Parse, node => node.Name != null && node.Name.EndsWith(".json")),
            (YamlConfigurationParser.Parse, node => node.Name != null && (node.Name.EndsWith(".yaml") || node.Name.EndsWith("yml")))
        }
    });