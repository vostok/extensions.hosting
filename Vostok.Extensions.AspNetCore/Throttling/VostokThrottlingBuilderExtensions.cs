using JetBrains.Annotations;

using Microsoft.AspNetCore.Http;

using Vostok.Throttling.Config;
using Vostok.Throttling.Quotas;

namespace Vostok.Extensions.AspNetCore.Throttling;

[PublicAPI]
public static class VostokThrottlingBuilderExtensions
{
    /// <summary>
    /// Disables the throttling middleware altogether.
    /// </summary>
    public static IVostokThrottlingBuilder DisableThrottling(this IVostokThrottlingBuilder configurator) =>
        configurator.ConfigureMiddleware(settings => settings.Enabled = _ => false);

    /// <summary>
    /// Disables generation of throttling metrics.
    /// </summary>
    public static IVostokThrottlingBuilder DisableMetrics(this IVostokThrottlingBuilder configurator) =>
        configurator.ConfigureOptions(s => s.EnableMetrics = false);

    /// <summary>
    /// <para>If set to <c>true</c>, adds an instance of <see cref="ThreadPoolOverloadQuota"/> to the throttling provider.</para>
    /// </summary>
    public static IVostokThrottlingBuilder DisableThreadPoolOverloadQuota(this IVostokThrottlingBuilder configurator) =>
        configurator.ConfigureOptions(s => s.UseThreadPoolOverloadQuota = false);

    /// <summary>
    /// <para>Configures throttling essentials, such as capacity and queue limits, to be obtained from given <paramref name="essentialsProvider"/>.</para>
    /// <para>See <see cref="ThrottlingEssentials"/> for more info.</para>
    /// </summary>
    public static IVostokThrottlingBuilder UseEssentials(this IVostokThrottlingBuilder configurator, Func<ThrottlingEssentials> essentialsProvider) =>
        configurator.ConfigureBuilder(builder => builder.UseEssentials(essentialsProvider));

    /// <inheritdoc cref="ThrottlingConfigurationBuilderExtensions.UsePropertyQuota"/>
    public static IVostokThrottlingBuilder UsePropertyQuota(this IVostokThrottlingBuilder configurator, string propertyName, Func<PropertyQuotaOptions> quotaOptionsProvider) =>
        configurator.ConfigureBuilder(builder => builder.UsePropertyQuota(propertyName, quotaOptionsProvider));

    /// <inheritdoc cref="ThrottlingConfigurationBuilderExtensions.UseCustomQuota"/>
    public static IVostokThrottlingBuilder UseCustomQuota(this IVostokThrottlingBuilder configurator, IThrottlingQuota quota) =>
        configurator.ConfigureBuilder(builder => builder.UseCustomQuota(quota));

    /// <inheritdoc cref="ThrottlingConfigurationBuilderExtensions.UseConsumerQuota"/>
    public static IVostokThrottlingBuilder UseConsumerQuota(this IVostokThrottlingBuilder configurator, Func<PropertyQuotaOptions> quotaOptionsProvider) =>
        configurator
            .ConfigureMiddleware(settings => settings.UseConsumerQuota())
            .ConfigureBuilder(builder => builder.UseConsumerQuota(quotaOptionsProvider));

    /// <inheritdoc cref="ThrottlingConfigurationBuilderExtensions.UsePriorityQuota"/>
    public static IVostokThrottlingBuilder UsePriorityQuota(this IVostokThrottlingBuilder configurator, Func<PropertyQuotaOptions> quotaOptionsProvider) =>
        configurator
            .ConfigureMiddleware(settings => settings.UsePriorityQuota())
            .ConfigureBuilder(builder => builder.UsePriorityQuota(quotaOptionsProvider));

    /// <inheritdoc cref="ThrottlingConfigurationBuilderExtensions.UseMethodQuota"/>
    public static IVostokThrottlingBuilder UseMethodQuota(this IVostokThrottlingBuilder configurator, Func<PropertyQuotaOptions> quotaOptionsProvider) =>
        configurator
            .ConfigureMiddleware(settings => settings.UseMethodQuota())
            .ConfigureBuilder(builder => builder.UseMethodQuota(quotaOptionsProvider));

    /// <inheritdoc cref="ThrottlingConfigurationBuilderExtensions.UseUrlQuota"/>
    public static IVostokThrottlingBuilder UseUrlQuota(this IVostokThrottlingBuilder configurator, Func<PropertyQuotaOptions> quotaOptionsProvider) =>
        configurator
            .ConfigureMiddleware(settings => settings.UseUrlQuota())
            .ConfigureBuilder(builder => builder.UseUrlQuota(quotaOptionsProvider));

    /// <inheritdoc cref="ThrottlingConfigurationBuilderExtensions.UseCustomPropertyQuota"/>
    public static IVostokThrottlingBuilder UseCustomPropertyQuota(this IVostokThrottlingBuilder configurator, string propertyName, Func<HttpContext, string> propertyValueProvider, Func<PropertyQuotaOptions> quotaOptionsProvider) =>
        configurator
            .ConfigureMiddleware(settings => settings.UseCustomPropertyQuota(propertyName, propertyValueProvider))
            .ConfigureBuilder(builder => builder.UseCustomPropertyQuota(propertyName, quotaOptionsProvider));
}