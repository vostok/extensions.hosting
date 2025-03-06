using Vostok.Extensions.AspNetCore.Middlewares;

namespace Vostok.Extensions.AspNetCore.Builder;

/// <summary>
/// Represents additional configuration of <see cref="ThrottlingMiddleware"/>.
/// </summary>
internal sealed class VostokThrottlingOptions
{
    /// <inheritdoc cref="IVostokThrottlingBuilder.UseThreadPoolOverloadQuota"/>
    public bool UseThreadPoolOverloadQuota { get; set; } = true;

    /// <summary>
    /// <para>If set to <c>true</c>, sends throttling metrics.</para>
    /// </summary>
    public bool EnableMetrics { get; set; } = true;
}