using Microsoft.Extensions.DependencyInjection;

using OpenTelemetry.Resources;

using Vostok.Clusterclient.Core;
using Vostok.Commons.Environment;
using Vostok.ServiceDiscovery.Abstractions;

namespace Vostok.Extensions.Hosting.OpenTelemetry.ResourceDetectors;

internal sealed class ServiceResourceDetector(IServiceProvider provider) : IResourceDetector
{
    private readonly IReplicaInfo? _replicaInfo = provider.GetService<IServiceBeacon>()?.ReplicaInfo;

    public Resource Detect()
    {
        // todo: configure ClusterClientDefaults.ClientApplicationName
        var service = _replicaInfo?.Application ?? ClusterClientDefaults.ClientApplicationName;
        var environment = _replicaInfo?.Environment;

        List<KeyValuePair<string, object>> attributes = [new(SemanticConventions.AttributeHostName, EnvironmentInfo.Host)];
        if (environment != null)
            attributes.Add(new(SemanticConventions.AttributeDeploymentEnvironmentName, environment));

        return ResourceBuilder.CreateEmpty()
                              .AddService(service, autoGenerateServiceInstanceId: false)
                              .AddAttributes(attributes)
                              .Build();
    }
}