using JetBrains.Annotations;

using Vostok.Extensions.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Requirements;
using Vostok.Hosting.Houston.Abstractions;
using Vostok.Hosting.Houston.Abstractions.Requirements;

namespace Vostok.Extensions.Hosting.Helpers;

/// <summary>
/// Provides helper methods to detect Houston feature requirements of configurators.
/// </summary>
[PublicAPI]
public static class HoustonRequirementDetector
{
    public static bool RequiresBinariesInfo(IVostokHostConfigurator configurator)
        => RequiresBinariesInfoInternal(() => RequirementDetector.GetRequiredHostExtensions(configurator));

    public static bool RequiresAssets(IVostokHostConfigurator configurator)
        => RequiresAssetsInternal(() => RequirementDetector.GetRequiredHostExtensions(configurator));

    public static Type? GetRequiredStateType(IVostokHostConfigurator configurator)
        => GetRequiredStateTypeInternal(configurator.GetType(),
            () => RequirementDetector.GetRequiredHostExtensions(configurator));

    public static IEnumerable<string> GetRequiredAssetNames(IVostokHostConfigurator configurator)
        => GetRequiredAssetNamesInternal(() => RequirementDetector.GetRequiredHostExtensions(configurator));

    private static bool RequiresBinariesInfoInternal(Func<IEnumerable<RequiresHostExtension>> getRequiredExtensions)
        => getRequiredExtensions().Any(r => r is RequiresHoustonBinariesInfo);

    private static bool RequiresAssetsInternal(Func<IEnumerable<RequiresHostExtension>> getRequiredExtensions)
        => getRequiredExtensions().Any(r => r is RequiresHoustonAssets);

    private static Type? GetRequiredStateTypeInternal(Type configuratorType,
        Func<IEnumerable<RequiresHostExtension>> getRequiredExtensions)
    {
        var stateTypes = getRequiredExtensions()
            .Where(e => e.Type.IsGenericType && e.Type.GetGenericTypeDefinition() == typeof(IHoustonStateManager<>))
            .Select(e => e.Type.GetGenericArguments()[0])
            .Distinct()
            .ToList();

        if (stateTypes.Count > 1)
            throw new ArgumentException(
                $"Configurator of type '{configuratorType.Name}' requires more than one state type: {string.Join(", ", stateTypes)}.");

        return stateTypes.FirstOrDefault();
    }

    private static IEnumerable<string> GetRequiredAssetNamesInternal(
        Func<IEnumerable<RequiresHostExtension>> getRequiredExtensions)
    {
        return getRequiredExtensions()
            .Select(e => e as RequiresHoustonAsset)
            .Where(e => e != null)
            .Select(e => e!.Name)
            .Distinct();
    }
}