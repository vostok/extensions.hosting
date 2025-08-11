using Vostok.Extensions.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Requirements;

using JetBrains.Annotations;

namespace Vostok.Extensions.Hosting.Helpers;

/// <summary>
/// Provides helper methods to detect application requirements, such as <see cref="RequiresConfiguration"/> or <see cref="RequiresHostExtension"/>.
/// </summary>
[PublicAPI]
public static class RequirementDetector
{
    public static bool RequiresPort(IVostokHostConfigurator configurator)
        => RequirementAttributesHelper.GetAttributes<RequiresPort>(configurator).Any();

    public static IEnumerable<RequiresHostExtension> GetRequiredHostExtensions(IVostokHostConfigurator configurator)
        => RequirementAttributesHelper.GetAttributes<RequiresHostExtension>(configurator);
}