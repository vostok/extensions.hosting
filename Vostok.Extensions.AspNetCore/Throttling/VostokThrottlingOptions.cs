using Vostok.Extensions.AspNetCore.Middlewares.Throttling;
using Vostok.Throttling.Quotas;

namespace Vostok.Extensions.AspNetCore.Throttling;

/// <summary>
/// Represents additional configuration of <see cref="ThrottlingMiddleware"/>.
/// </summary>
internal sealed class VostokThrottlingOptions
{
    /// <summary>
    /// <para>If set to <c>true</c>, adds an instance of <see cref="ThreadPoolOverloadQuota"/> to the throttling provider.</para>
    /// </summary>
    public bool UseThreadPoolOverloadQuota { get; set; } = true;

    /// <summary>
    /// <para>If set to <c>true</c>, sends throttling metrics.</para>
    /// </summary>
    public bool EnableMetrics { get; set; } = true;

    /// <summary>
    /// <para>If set to <c>true</c>, enables throttling diagnostic info provider.</para>
    /// </summary>
    public bool AddInfoProvider { get; set; } = true;

    /// <summary>
    /// <para>If set to <c>true</c>, enables throttling health checks.</para>
    /// </summary>
    public bool AddHealthCheck { get; set; } = true;
}