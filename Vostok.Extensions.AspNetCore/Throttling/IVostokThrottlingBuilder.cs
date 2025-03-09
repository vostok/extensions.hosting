using JetBrains.Annotations;

using Vostok.Extensions.AspNetCore.Middlewares.Throttling;
using Vostok.Throttling.Config;
using Vostok.Throttling.Metrics;

namespace Vostok.Extensions.AspNetCore.Throttling;

/// <summary>
/// Represents <see cref="ThrottlingMiddleware"/> configurator.
/// </summary>
[PublicAPI]
public interface IVostokThrottlingBuilder
{
    /// <summary>
    /// <para>Configuration of application's throttling metrics.</para>
    /// </summary>
    IVostokThrottlingBuilder ConfigureMetrics(Action<ThrottlingMetricsOptions> configure);

    /// <summary>
    /// Allows to customize advanced options pertinent to internal middleware responsible for request throttling.
    /// </summary>
    IVostokThrottlingBuilder ConfigureMiddleware(Action<ThrottlingMiddlewareOptions> configure);

    /// <summary>
    /// Allows to customize <see cref="ThrottlingConfigurationBuilder"/>.
    /// </summary>
    IVostokThrottlingBuilder ConfigureBuilder(Action<ThrottlingConfigurationBuilder> configure);

    /// <summary>
    /// Allows to customize <see cref="VostokThrottlingOptions"/>.
    /// </summary>
    internal IVostokThrottlingBuilder ConfigureOptions(Action<VostokThrottlingOptions> configure);
}