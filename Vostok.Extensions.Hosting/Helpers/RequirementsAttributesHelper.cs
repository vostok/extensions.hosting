using System.Reflection;

using Vostok.Extensions.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.Helpers;

internal static class RequirementAttributesHelper
{
    public static IEnumerable<TRequirement> GetAttributes<TRequirement>(IVostokHostConfigurator configurator)
        where TRequirement : Attribute
        => GetAttributes<TRequirement>(configurator.GetType());

    public static IEnumerable<TRequirement> GetAttributes<TRequirement>(Type configuratorType)
        where TRequirement : Attribute
        => GetDirectAttributes<TRequirement>(configuratorType)
            .Concat(GetStaticAttributes<TRequirement>(configuratorType));

    private static IEnumerable<TRequirement> GetDirectAttributes<TRequirement>(Type configuratorType)
        where TRequirement : Attribute
        => configuratorType?.GetCustomAttributes<TRequirement>(true) ?? Enumerable.Empty<TRequirement>();

    private static IEnumerable<TRequirement> GetStaticAttributes<TRequirement>(Type configuratorType)
        where TRequirement : Attribute
    {
        var property = configuratorType.GetProperty("AdditionalRequirements",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy,
            null, typeof(IEnumerable<Attribute>), Array.Empty<Type>(), null);

        return property == null
            ? Enumerable.Empty<TRequirement>()
            : ((IEnumerable<Attribute>)property.GetValue(null)).Where(attribute => attribute is TRequirement)
            .Cast<TRequirement>();
    }
}