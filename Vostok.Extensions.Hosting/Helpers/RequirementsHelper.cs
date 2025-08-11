using JetBrains.Annotations;

using Vostok.Commons.Helpers.Network;
using Vostok.Extensions.Hosting.Abstractions;
using Vostok.Extensions.Hosting.Components.ServiceDiscovery;
using Vostok.Hosting.Abstractions.Requirements;

namespace Vostok.Extensions.Hosting.Helpers;

/// <summary>
/// A collection of helper methods to aid in satisfying configurator requirements when configuring a <see cref="IVostokHostingEnvironment"/>.
/// </summary>
[PublicAPI]
public static class RequirementsHelper
{
    //todo: где именно вызывать?
    public static void EnsurePort(IVostokHostConfigurator application, IServiceBeaconBuilder builder)
    {
        if (RequirementDetector.RequiresPort(application))
            builder.SetPort(FreeTcpPortFinder.GetFreePort());
    }
}