using System.Reflection;

using Vostok.Extensions.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.Helpers;

internal static class RequirementAttributesHelper
{
    public static IEnumerable<TRequirement> GetAttributes<TRequirement>(IVostokHostConfigurator application)
        where TRequirement : Attribute
        => GetAttributes<TRequirement>(application.GetType());

    public static IEnumerable<TRequirement> GetAttributes<TRequirement>(Type applicationType)
        where TRequirement : Attribute
        => GetDirectAttributes<TRequirement>(applicationType).Concat(GetStaticAttributes<TRequirement>(applicationType));

    private static IEnumerable<TRequirement> GetDirectAttributes<TRequirement>(Type applicationType)
        where TRequirement : Attribute
        => applicationType?.GetCustomAttributes<TRequirement>(true) ?? Enumerable.Empty<TRequirement>();

    private static IEnumerable<TRequirement> GetStaticAttributes<TRequirement>(Type applicationType)
        where TRequirement : Attribute
    {
        var property = applicationType.GetProperty("AdditionalRequirements", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy,
            null, typeof(IEnumerable<Attribute>), Array.Empty<Type>(), null);

        return property == null
            ? Enumerable.Empty<TRequirement>()
            : ((IEnumerable<Attribute>)property.GetValue(null)).Where(attribute => attribute is TRequirement).Cast<TRequirement>();
    }
}