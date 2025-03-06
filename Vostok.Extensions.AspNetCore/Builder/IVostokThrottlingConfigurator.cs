using JetBrains.Annotations;

using Vostok.Extensions.AspNetCore.Middlewares;
using Vostok.Throttling.Config;
using Vostok.Throttling.Metrics;

namespace Vostok.Extensions.AspNetCore.Builder;

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
    /// Allows to customize advanced settings pertinent to internal middleware responsible for request throttling.
    /// </summary>
    IVostokThrottlingBuilder ConfigureMiddleware(Action<ThrottlingSettings> configure);

    /// <summary>
    /// Allows to customize <see cref="ThrottlingConfigurationBuilder"/>.
    /// </summary>
    IVostokThrottlingBuilder ConfigureBuilder(Action<ThrottlingConfigurationBuilder> configure);

    /// <summary>
    /// Allows to customize <see cref="VostokThrottlingOptions"/>.
    /// </summary>
    internal IVostokThrottlingBuilder ConfigureOptions(Action<VostokThrottlingOptions> configure);
}