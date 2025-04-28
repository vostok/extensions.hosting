using Microsoft.Extensions.DependencyInjection;

using Vostok.Commons.Environment;
using Vostok.Hosting.Abstractions;
using Vostok.ServiceDiscovery.Abstractions;

namespace Vostok.Extensions.Hosting.Helpers;

internal sealed record ServiceNameProvider(string ServiceName)
{
    public static ServiceNameProvider Create(IServiceProvider provider)
    {
        var serviceBeacon = provider.GetService<IServiceBeacon>();
        var identity = provider.GetService<IVostokApplicationIdentity>();

        var serviceName = serviceBeacon?.ReplicaInfo.Application ??
                          identity?.FormatServiceName() ??
                          EnvironmentInfo.Application;

        return new ServiceNameProvider(serviceName);
    }
}