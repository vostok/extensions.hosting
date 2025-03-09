using JetBrains.Annotations;

using Vostok.Extensions.AspNetCore.Diagnostics;
using Vostok.Extensions.AspNetCore.Middlewares;
using Vostok.Extensions.AspNetCore.Middlewares.DatacentersAwareness;
using Vostok.Extensions.AspNetCore.Middlewares.DiagnosticApi;
using Vostok.Extensions.AspNetCore.Middlewares.DistributedContext;
using Vostok.Extensions.AspNetCore.Middlewares.FillRequestInfo;
using Vostok.Extensions.AspNetCore.Middlewares.HttpContextTweak;
using Vostok.Extensions.AspNetCore.Middlewares.Logging;
using Vostok.Extensions.AspNetCore.Middlewares.PingApi;
using Vostok.Extensions.AspNetCore.Middlewares.Throttling;
using Vostok.Extensions.AspNetCore.Middlewares.UnhandledException;
using Vostok.Extensions.AspNetCore.Throttling;

namespace Vostok.Extensions.AspNetCore;

/// <summary>
/// Vostok middlewares builder.
/// </summary>
[PublicAPI]
public interface IVostokMiddlewaresBuilder
{
    /// <summary>
    /// Customizes built-in <see cref="HttpContextTweakMiddleware"/>.
    /// </summary>
    IVostokMiddlewaresBuilder ConfigureHttpContextTweaks(Action<HttpContextTweakMiddlewareOptions> configure);

    /// <summary>
    /// Customizes built-in <see cref="FillRequestInfoMiddleware"/>.
    /// </summary>
    IVostokMiddlewaresBuilder ConfigureRequestInfoFilling(Action<FillRequestInfoMiddlewareOptions> configure);

    /// <summary>
    /// Customizes built-in <see cref="DistributedContextMiddleware"/>.
    /// </summary>
    IVostokMiddlewaresBuilder ConfigureDistributedContext(Action<DistributedContextMiddlewareOptions> configure);

    /// <summary>
    /// Customizes built-in <see cref="ThrottlingMiddleware"/>.
    /// </summary>
    IVostokMiddlewaresBuilder ConfigureThrottling(Action<IVostokThrottlingBuilder> configure);

    /// <summary>
    /// Customizes built-in <see cref="LoggingMiddleware"/>.
    /// </summary>
    IVostokMiddlewaresBuilder ConfigureLogging(Action<LoggingMiddlewareOptions> configure);

    /// <summary>
    /// Customizes built-in <see cref="DatacenterAwarenessMiddleware"/>.
    /// </summary>
    IVostokMiddlewaresBuilder ConfigureDatacenterAwareness(Action<DatacenterAwarenessMiddlewareOptions> configure);

    /// <summary>
    /// Customizes built-in <see cref="UnhandledExceptionMiddleware"/>.
    /// </summary>
    IVostokMiddlewaresBuilder ConfigureUnhandledExceptions(Action<UnhandledExceptionMiddlewareOptions> configure);

    /// <summary>
    /// Customizes built-in <see cref="PingApiMiddleware"/>.
    /// </summary>
    IVostokMiddlewaresBuilder ConfigurePingApi(Action<PingApiMiddlewareOptions> configure);

    /// <summary>
    /// Customizes built-in <see cref="DiagnosticApiMiddleware"/>.
    /// </summary>
    IVostokMiddlewaresBuilder ConfigureDiagnosticApi(Action<DiagnosticApiMiddlewareOptions> configure);

    /// <summary>
    /// Customizes built-in diagnostic features (info providers and health checks).
    /// </summary>
    IVostokMiddlewaresBuilder ConfigureDiagnosticFeatures(Action<DiagnosticFeaturesSettings> configure);

    /// <summary>
    /// Allows to customize <see cref="VostokMiddlewaresOptions"/>.
    /// </summary>
    internal IVostokMiddlewaresBuilder Configure(Action<VostokMiddlewaresOptions> configure);
}