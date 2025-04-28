using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.DependencyInjection;

using OpenTelemetry.Resources;

using Vostok.Commons.Environment;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.ServiceDiscovery.Abstractions;

using static Vostok.Extensions.Hosting.OpenTelemetry.VostokSemanticConventions;

namespace Vostok.Extensions.Hosting.OpenTelemetry.ResourceDetectors;

[SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeNotEvident")]
internal sealed class ServiceResourceDetector(IServiceProvider provider) : IResourceDetector
{
    public Resource Detect()
    {
        var service = provider.GetRequiredService<ServiceNameProvider>().ServiceName;
        var environment = provider.GetService<IServiceBeacon>()?.ReplicaInfo?.Environment;

        List<KeyValuePair<string, object>> attributes = [new(AttributeHostName, EnvironmentInfo.Host)];

        if (environment != null)
            attributes.Add(new(AttributeDeploymentEnvironmentName, environment));

        return ResourceBuilder.CreateEmpty()
                              .AddService(service, autoGenerateServiceInstanceId: false)
                              .AddAttributes(attributes)
                              .Build();
    }
}