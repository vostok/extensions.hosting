using JetBrains.Annotations;

using Microsoft.AspNetCore.Http;

using Vostok.Extensions.AspNetCore.Middlewares.Throttling;

namespace Vostok.Extensions.AspNetCore.Throttling;

[PublicAPI]
public static class ThrottlingOptionsExtensions
{
    /// <summary>
    /// <inheritdoc cref="ThrottlingConfigurationBuilderExtensions.UseConsumerQuota"/>
    /// </summary>
    public static ThrottlingMiddlewareOptions UseConsumerQuota(this ThrottlingMiddlewareOptions options)
    {
        options.AddConsumerProperty = true;
        return options;
    }

    /// <summary>
    /// <inheritdoc cref="ThrottlingConfigurationBuilderExtensions.UsePriorityQuota"/>
    /// </summary>
    public static ThrottlingMiddlewareOptions UsePriorityQuota(this ThrottlingMiddlewareOptions options)
    {
        options.AddPriorityProperty = true;
        return options;
    }

    /// <summary>
    /// <inheritdoc cref="ThrottlingConfigurationBuilderExtensions.UseMethodQuota"/>
    /// </summary>
    public static ThrottlingMiddlewareOptions UseMethodQuota(this ThrottlingMiddlewareOptions options)
    {
        options.AddMethodProperty = true;
        return options;
    }

    /// <summary>
    /// <inheritdoc cref="ThrottlingConfigurationBuilderExtensions.UseUrlQuota"/>
    /// </summary>
    public static ThrottlingMiddlewareOptions UseUrlQuota(this ThrottlingMiddlewareOptions options)
    {
        options.AddUrlProperty = true;
        return options;
    }

    /// <summary>
    /// <inheritdoc cref="ThrottlingConfigurationBuilderExtensions.UseCustomPropertyQuota"/>
    /// </summary>
    public static ThrottlingMiddlewareOptions UseCustomPropertyQuota(this ThrottlingMiddlewareOptions options, string propertyName, Func<HttpContext, string> propertyValueProvider)
    {
        options.AdditionalProperties.Add(context => (propertyName, propertyValueProvider(context)));
        return options;
    }
}