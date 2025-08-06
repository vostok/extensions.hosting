using JetBrains.Annotations;

using Vostok.Extensions.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Requirements;
using Vostok.Hosting.Houston.Abstractions;
using Vostok.Hosting.Houston.Abstractions.Requirements;

namespace Vostok.Extensions.Hosting.Helpers;

    /// <summary>
    /// Provides helper methods to detect Houston feature requirements of applications.
    /// </summary>
    [PublicAPI]
    public static class HoustonRequirementDetector
    {
        public static bool RequiresBinariesInfo(IVostokHostConfigurator configurator)
            => RequiresBinariesInfoInternal(() => RequirementDetector.GetRequiredHostExtensions(configurator));

        public static bool RequiresAssets(IVostokHostConfigurator application)
            => RequiresAssetsInternal(() => RequirementDetector.GetRequiredHostExtensions(application));

        public static Type? GetRequiredStateType(IVostokHostConfigurator application)
            => GetRequiredStateTypeInternal(application.GetType(), () => RequirementDetector.GetRequiredHostExtensions(application));

        public static IEnumerable<string> GetRequiredAssetNames(IVostokHostConfigurator application)
            => GetRequiredAssetNamesInternal(() => RequirementDetector.GetRequiredHostExtensions(application));
        
        private static bool RequiresBinariesInfoInternal(Func<IEnumerable<RequiresHostExtension>> getRequiredExtensions)
            => getRequiredExtensions().Any(r => r is RequiresHoustonBinariesInfo);

        private static bool RequiresAssetsInternal(Func<IEnumerable<RequiresHostExtension>> getRequiredExtensions)
            => getRequiredExtensions().Any(r => r is RequiresHoustonAssets);

        private static Type? GetRequiredStateTypeInternal(Type applicationType, Func<IEnumerable<RequiresHostExtension>> getRequiredExtensions)
        {
            var stateTypes = getRequiredExtensions()
                .Where(e => e.Type.IsGenericType && e.Type.GetGenericTypeDefinition() == typeof(IHoustonStateManager<>))
                .Select(e => e.Type.GetGenericArguments()[0])
                .Distinct()
                .ToList();

            if (stateTypes.Count > 1)
                throw new ArgumentException($"Application of type '{applicationType.Name}' requires more than one state type: {string.Join(", ", stateTypes)}.");

            return stateTypes.FirstOrDefault();
        }

        private static IEnumerable<string> GetRequiredAssetNamesInternal(Func<IEnumerable<RequiresHostExtension>> getRequiredExtensions) =>
            getRequiredExtensions()
                .Select(e => e as RequiresHoustonAsset)
                .Where(e => e != null)
                .Select(e => e!.Name)
                .Distinct();
    }