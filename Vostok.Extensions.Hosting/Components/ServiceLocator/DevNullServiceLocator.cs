using Vostok.ServiceDiscovery.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ServiceLocator;

internal sealed class DevNullServiceLocator : IServiceLocator
{
    public IServiceTopology? Locate(string environment, string application) => null;
}