using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Vostok.Extensions.Hosting.Abstractions;
using Vostok.Hosting.Houston.Abstractions;

namespace Vostok.Extensions.Hosting.Helpers;

/// <summary>
/// A helper that allows to check whether given instance of <see cref="IServiceProvider"/> satisfies all of the configurator's Houston requirements expressed via attributes.
/// </summary>
[PublicAPI]
public static class HoustonRequirementsChecker
{
    /// <summary>
    /// Checks whether given <paramref name="serviceProvider"/> satisfies all Houston requirements imposed by <paramref name="configurator"/>.
    /// </summary>
    /// <exception cref="RequirementsCheckException">Some of configurator's requirements were not satisfied.</exception>
    public static void Check(
        IVostokHostConfigurator configurator,
        IServiceProvider serviceProvider)
    {
        if (!TryCheck(configurator, serviceProvider, out var errors))
            throw new RequirementsCheckException(configurator.GetType(), errors);
    }

    /// <summary>
    /// Checks whether given <paramref name="serviceProvider"/> satisfies all Houston requirements imposed by <paramref name="configurator"/>.
    /// </summary>
    /// <returns><c>true</c> if all requirements are met, <c>false</c> otherwise (with at least one element in <paramref name="errors"/>).</returns>
    public static bool TryCheck(
        IVostokHostConfigurator configurator,
        IServiceProvider serviceProvider,
        out List<string> errors)
    {
        RequirementsChecker.TryCheck(configurator, serviceProvider, out errors);

        CheckAssets(configurator, serviceProvider, errors);

        return errors.Count == 0;
    }

    private static void CheckAssets(IVostokHostConfigurator configurator, IServiceProvider serviceProvider,
        List<string> errors)
    {
        var requiredAssets = HoustonRequirementDetector.GetRequiredAssetNames(configurator);

        foreach (var requiredAsset in requiredAssets)
            CheckAsset(serviceProvider, requiredAsset, errors);
    }

    private static void CheckAsset(IServiceProvider serviceProvider, string assetName, List<string> errors)
    {
        try
        {
            serviceProvider.GetRequiredService<IHoustonAssets>().Get(assetName);
        }
        catch (Exception error)
        {
            var assets = serviceProvider.GetService<IHoustonAssets>();

            var available = assets != null ? assets.GetAll() : Array.Empty<HoustonAsset>();
            var availableStr = available.Any() ? string.Join(", ", available) : "none";

            errors.Add(
                $"Configurator requires an asset with name '{assetName}', which has not been provided by the host.\n" +
                $"Available assets: {availableStr}.\n" +
                (error.InnerException?.Message ?? error.Message));
        }
    }
}