using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Vostok.Extensions.Hosting.Abstractions;
using Vostok.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Requirements;
using Vostok.ServiceDiscovery.Abstractions;

namespace Vostok.Extensions.Hosting.Helpers;

/// <summary>
/// A helper that allows to check whether given instance of <see cref="IVostokHostingEnvironment"/> satisfies all of the application's requirements expressed via attributes.
/// </summary>
[PublicAPI]
public static class RequirementsChecker
{
    /// <summary>
    /// Checks whether given <paramref name="environment"/> satisfies all requirements imposed by <paramref name="application"/>.
    /// </summary>
    /// <exception cref="RequirementsCheckException">Some of application's requirements were not satisfied.</exception>
    public static void Check(
        IVostokHostConfigurator configurator,
        IServiceProvider serviceProvider)
    {
        if (!TryCheck(configurator, serviceProvider, out var errors))
            throw new RequirementsCheckException(configurator.GetType(), errors);
    }

    /// <summary>
    /// Checks whether given <paramref name="environment"/> satisfies all requirements imposed by <paramref name="application"/>.
    /// </summary>
    /// <returns><c>true</c> if all requirements are met, <c>false</c> otherwise (with at least one element in <paramref name="errors"/>).</returns>
    public static bool TryCheck(
        IVostokHostConfigurator configurator,
        IServiceProvider serviceProvider,
        out List<string> errors)
    {
        if (configurator == null)
            throw new ArgumentNullException(nameof(configurator));

        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        errors = new List<string>();

        CheckPort(configurator, serviceProvider, errors);
        CheckHostExtensions(configurator, serviceProvider, errors);

        return errors.Count == 0;
    }

    #region CheckPort

    private static void CheckPort(IVostokHostConfigurator configurator, IServiceProvider serviceProvider,
        List<string> errors)
        => CheckPortInternal(() => RequirementDetector.RequiresPort(configurator), serviceProvider, errors);

    private static void CheckPortInternal(
        Func<bool> requiresPort,
        IServiceProvider serviceProvider,
        List<string> errors)
    {
        if (!requiresPort())
            return;

        var serviceBeacon = serviceProvider.GetService<IServiceBeacon>();

        if (serviceBeacon is null || !serviceBeacon.ReplicaInfo.TryGetUrl(out var url))
            return;

        if (url?.Port == null)
            errors.Add(
                "Application requires a port, which is not provided by host (see 'SetPort' extension when configuring hosting environment).");
    }

    #endregion

    #region CheckHostExtensions

    private static void CheckHostExtensions(IVostokHostConfigurator application, IServiceProvider serviceProvider,
        List<string> errors)
        => CheckHostExtensionsInternal(() => RequirementDetector.GetRequiredHostExtensions(application),
            serviceProvider,
            errors);

    private static void CheckHostExtensionsInternal(
        Func<IEnumerable<RequiresHostExtension>> getRequiredHostExtensions,
        IServiceProvider serviceProvider,
        List<string> errors)
    {
        foreach (var requiredExtension in getRequiredHostExtensions().Where(h => h.Key == null))
        {
            if (serviceProvider.GetService(requiredExtension.Type) == null)
                errors.Add(
                    $"Application requires a service of type '{requiredExtension.Type}', which is not registered by host");
        }

        foreach (var requiredExtension in getRequiredHostExtensions().Where(h => h.Key != null))
        {
            if (serviceProvider.GetKeyedService(requiredExtension.Type, requiredExtension.Key!) == null)
                errors.Add(
                    $"Application requires a service of type '{requiredExtension.Type}' with key '{requiredExtension.Key}', which is not registered by host");
        }
    }

    #endregion
}