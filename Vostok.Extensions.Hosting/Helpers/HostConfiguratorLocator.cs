using System.Reflection;

using JetBrains.Annotations;

using Vostok.Extensions.Hosting.Abstractions;
using Vostok.Hosting.Abstractions;
using Vostok.Hosting.Houston.Abstractions;

namespace Vostok.Extensions.Hosting.Helpers;

    [PublicAPI]
    public static class HostConfiguratorLocator
    {
        public static IVostokHostConfigurator LocateHostConfigurator(Assembly assembly, Type configuratorInterface, string[] args)
        {
            var configuratorType = SelectEntryPoint(assembly, args).ApplicationType;
            
            if (!configuratorType.GetInterfaces().Contains(configuratorInterface))
                throw new HostConfiguratorLocatorException($"Configurator not implementing required interface: {configuratorInterface.Name}");
            
            var configuratorInstance = CreateConfiguratorInstance(configuratorType);

            if (configuratorInstance is IVostokHostConfigurator vostokApplication)
                return vostokApplication;

            throw new HostConfiguratorLocatorException($"Selected configurator '{configuratorType.Name}' is not an {nameof(IVostokHostConfigurator)}");
        }

        public static HoustonEntryPointAttribute SelectEntryPoint(Assembly assembly, string[] args)
        {
            var entryPoints = assembly.GetCustomAttributes<HoustonEntryPointAttribute>().ToArray();

            if (entryPoints.Length == 0)
                throw new HostConfiguratorLocatorException($"There are no {nameof(HoustonEntryPointAttribute)}s in the assembly '{assembly.GetName().Name}'.");

            if (entryPoints.Length == 1)
                return entryPoints.Single();

            var matchingEntryPoints = entryPoints.Where(entry => ArgumentsMatch(entry, args)).ToArray();
            if (matchingEntryPoints.Length == 0)
                throw new HostConfiguratorLocatorException($"None of the {nameof(HoustonEntryPointAttribute)}s in the assembly '{assembly.GetName().Name}' match given input arguments: '{string.Join(" ", args)}' (matching is performed on the first argument).");

            if (matchingEntryPoints.Length > 1)
                throw new HostConfiguratorLocatorException($"Multiple {nameof(HoustonEntryPointAttribute)}s ({string.Join(", ", matchingEntryPoints.Select(e => e.ApplicationType.Name))}) in the assembly '{assembly.GetName().Name}' match given input arguments: '{string.Join(" ", args)}' (matching is performed on the first argument).");

            return matchingEntryPoints.Single();
        }

        private static bool ArgumentsMatch(HoustonEntryPointAttribute entryPoint, string[] args)
        {
            var firstEntryPointArgument = entryPoint.Arguments?.Split(null as char[], StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            var firstActualArgument = args.FirstOrDefault();

            return StringComparer.OrdinalIgnoreCase.Equals(firstEntryPointArgument, firstActualArgument);
        }

        private static object CreateConfiguratorInstance(Type applicationType)
        {
            if (!applicationType.IsValueType && applicationType.GetConstructor(Type.EmptyTypes) == null)
                throw new HostConfiguratorLocatorException($"Selected configurator type '{applicationType.Name}' does not have a parameterless constructor.");

            return Activator.CreateInstance(applicationType);
        }
    }