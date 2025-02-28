using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.DependencyInjection;

using OpenTelemetry.Resources;

using Vostok.Clusterclient.Core;
using Vostok.Commons.Environment;
using Vostok.ServiceDiscovery.Abstractions;

using static Vostok.Extensions.Hosting.OpenTelemetry.VostokSemanticConventions;

namespace Vostok.Extensions.Hosting.OpenTelemetry.ResourceDetectors;

[SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeNotEvident")]
internal sealed class ServiceResourceDetector(IServiceProvider provider) : IResourceDetector
{
    private readonly IReplicaInfo? _replicaInfo = provider.GetService<IServiceBeacon>()?.ReplicaInfo;

    public Resource Detect()
    {
        // todo: configure ClusterClientDefaults.ClientApplicationName
        var service = _replicaInfo?.Application ?? ClusterClientDefaults.ClientApplicationName;
        var environment = _replicaInfo?.Environment;

        List<KeyValuePair<string, object>> attributes = [new(AttributeHostName, EnvironmentInfo.Host)];

        if (environment != null)
            attributes.Add(new(AttributeDeploymentEnvironmentName, environment));

        return ResourceBuilder.CreateEmpty()
                              .AddService(service, autoGenerateServiceInstanceId: false)
                              .AddAttributes(attributes)
                              .Build();
    }
}