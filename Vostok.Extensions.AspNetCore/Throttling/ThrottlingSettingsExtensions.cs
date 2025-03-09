using JetBrains.Annotations;

using Microsoft.AspNetCore.Http;

using Vostok.Extensions.AspNetCore.Middlewares.Throttling;

namespace Vostok.Extensions.AspNetCore.Throttling;

[PublicAPI]
public static class ThrottlingSettingsExtensions
{
    /// <summary>
    /// <inheritdoc cref="ThrottlingConfigurationBuilderExtensions.UseConsumerQuota"/>
    /// </summary>
    public static ThrottlingSettings UseConsumerQuota(this ThrottlingSettings settings)
    {
        settings.AddConsumerProperty = true;
        return settings;
    }

    /// <summary>
    /// <inheritdoc cref="ThrottlingConfigurationBuilderExtensions.UsePriorityQuota"/>
    /// </summary>
    public static ThrottlingSettings UsePriorityQuota(this ThrottlingSettings settings)
    {
        settings.AddPriorityProperty = true;
        return settings;
    }

    /// <summary>
    /// <inheritdoc cref="ThrottlingConfigurationBuilderExtensions.UseMethodQuota"/>
    /// </summary>
    public static ThrottlingSettings UseMethodQuota(this ThrottlingSettings settings)
    {
        settings.AddMethodProperty = true;
        return settings;
    }

    /// <summary>
    /// <inheritdoc cref="ThrottlingConfigurationBuilderExtensions.UseUrlQuota"/>
    /// </summary>
    public static ThrottlingSettings UseUrlQuota(this ThrottlingSettings settings)
    {
        settings.AddUrlProperty = true;
        return settings;
    }

    /// <summary>
    /// <inheritdoc cref="ThrottlingConfigurationBuilderExtensions.UseCustomPropertyQuota"/>
    /// </summary>
    public static ThrottlingSettings UseCustomPropertyQuota(this ThrottlingSettings settings, string propertyName, Func<HttpContext, string> propertyValueProvider)
    {
        settings.AdditionalProperties.Add(context => (propertyName, propertyValueProvider(context)));
        return settings;
    }
}