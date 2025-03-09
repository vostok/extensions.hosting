using JetBrains.Annotations;

using Vostok.Throttling;
using Vostok.Throttling.Config;
using Vostok.Throttling.Quotas;

namespace Vostok.Extensions.AspNetCore.Throttling;

[PublicAPI]
public static class ThrottlingConfigurationBuilderExtensions
{
    /// <summary>
    /// <para>Configures throttling essentials, such as capacity and queue limits, to be obtained from given <paramref name="essentialsProvider"/>.</para>
    /// <para>See <see cref="ThrottlingEssentials"/> for more info.</para>
    /// </summary>
    public static ThrottlingConfigurationBuilder UseEssentials(this ThrottlingConfigurationBuilder builder, Func<ThrottlingEssentials> essentialsProvider) =>
        builder.SetEssentials(essentialsProvider);

    /// <summary>
    /// <para>Adds a quota for the property with given <paramref name="propertyName"/> and options fetched from <paramref name="quotaOptionsProvider"/>.</para>
    /// <para>Property quotas allow to explicitly limit parallelism of requests with some definite property values.</para>
    /// <para>By default, internal middleware adds all <see cref="WellKnownThrottlingProperties"/> except for the url:</para>
    /// <list type="bullet">
    ///     <item><description><see cref="WellKnownThrottlingProperties.Consumer"/></description></item>
    ///     <item><description><see cref="WellKnownThrottlingProperties.Priority"/></description></item>
    ///     <item><description><see cref="WellKnownThrottlingProperties.Method"/></description></item>
    /// </list>
    /// <para>When dealing with these well-known properties, it is recommended to use extensions over this method instead:</para>
    /// <list type="bullet">
    ///     <item><description><see cref="VostokThrottlingBuilderExtensions.UseConsumerQuota"/></description></item>
    ///     <item><description><see cref="VostokThrottlingBuilderExtensions.UsePriorityQuota"/></description></item>
    ///     <item><description><see cref="VostokThrottlingBuilderExtensions.UseMethodQuota"/></description></item>
    ///     <item><description><see cref="VostokThrottlingBuilderExtensions.UseUrlQuota"/></description></item>
    /// </list>
    /// </summary>
    public static ThrottlingConfigurationBuilder UsePropertyQuota(this ThrottlingConfigurationBuilder builder, string propertyName, Func<PropertyQuotaOptions> quotaOptionsProvider) =>
        builder.SetPropertyQuota(propertyName, quotaOptionsProvider);

    /// <summary>
    /// <para>Adds an instance of custom user-implemented quota.</para>
    /// <para>See <see cref="IThrottlingQuota"/> for more info.</para>
    /// </summary>
    public static ThrottlingConfigurationBuilder UseCustomQuota(this ThrottlingConfigurationBuilder builder, IThrottlingQuota quota) =>
        builder.AddCustomQuota(quota);

    /// <summary>
    /// <para>Sets up a quota on the <see cref="WellKnownThrottlingProperties.Consumer"/> request property configured by given <paramref name="quotaOptionsProvider"/>.</para>
    /// <para>See <see cref="UsePropertyQuota"/> for additional info on property quotas.</para>
    /// </summary>
    public static ThrottlingConfigurationBuilder UseConsumerQuota(this ThrottlingConfigurationBuilder builder, Func<PropertyQuotaOptions> quotaOptionsProvider) =>
        builder.UsePropertyQuota(WellKnownThrottlingProperties.Consumer, quotaOptionsProvider);

    /// <summary>
    /// <para>Sets up a quota on the <see cref="WellKnownThrottlingProperties.Priority"/> request property configured by given <paramref name="quotaOptionsProvider"/>.</para>
    /// <para>See <see cref="UsePropertyQuota"/> for additional info on property quotas.</para>
    /// </summary>
    public static ThrottlingConfigurationBuilder UsePriorityQuota(this ThrottlingConfigurationBuilder builder, Func<PropertyQuotaOptions> quotaOptionsProvider) =>
        builder.UsePropertyQuota(WellKnownThrottlingProperties.Priority, quotaOptionsProvider);

    /// <summary>
    /// <para>Sets up a quota on the <see cref="WellKnownThrottlingProperties.Method"/> request property configured by given <paramref name="quotaOptionsProvider"/>.</para>
    /// <para>See <see cref="UsePropertyQuota"/> for additional info on property quotas.</para>
    /// </summary>
    public static ThrottlingConfigurationBuilder UseMethodQuota(this ThrottlingConfigurationBuilder builder, Func<PropertyQuotaOptions> quotaOptionsProvider) =>
        builder.UsePropertyQuota(WellKnownThrottlingProperties.Method, quotaOptionsProvider);

    /// <summary>
    /// <para>Sets up a quota on the <see cref="WellKnownThrottlingProperties.Url"/> request property configured by given <paramref name="quotaOptionsProvider"/>.</para>
    /// <para>See <see cref="UsePropertyQuota"/> for additional info on property quotas.</para>
    /// </summary>
    public static ThrottlingConfigurationBuilder UseUrlQuota(this ThrottlingConfigurationBuilder builder, Func<PropertyQuotaOptions> quotaOptionsProvider) =>
        builder.UsePropertyQuota(WellKnownThrottlingProperties.Url, quotaOptionsProvider);

    /// <summary>
    /// <para>Sets up a quota on the <paramref name="propertyName"/> property configured by given <paramref name="quotaOptionsProvider"/>.</para>
    /// <para>See <see cref="UsePropertyQuota"/> for additional info on property quotas.</para>
    /// </summary>
    public static ThrottlingConfigurationBuilder UseCustomPropertyQuota(this ThrottlingConfigurationBuilder builder, string propertyName, Func<PropertyQuotaOptions> quotaOptionsProvider) =>
        builder.SetPropertyQuota(propertyName, quotaOptionsProvider);
}