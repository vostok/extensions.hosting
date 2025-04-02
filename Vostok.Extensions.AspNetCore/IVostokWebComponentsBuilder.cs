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
/// Vostok Web components builder.
/// </summary>
[PublicAPI]
public interface IVostokWebComponentsBuilder
{
    /// <summary>
    /// Customizes built-in <see cref="HttpContextTweakMiddleware"/>.
    /// </summary>
    IVostokWebComponentsBuilder ConfigureHttpContextTweaks(Action<HttpContextTweakMiddlewareOptions> configure);

    /// <summary>
    /// Customizes built-in <see cref="FillRequestInfoMiddleware"/>.
    /// </summary>
    IVostokWebComponentsBuilder ConfigureRequestInfoFilling(Action<FillRequestInfoMiddlewareOptions> configure);

    /// <summary>
    /// Customizes built-in <see cref="DistributedContextMiddleware"/>.
    /// </summary>
    IVostokWebComponentsBuilder ConfigureDistributedContext(Action<DistributedContextMiddlewareOptions> configure);

    /// <summary>
    /// Customizes built-in <see cref="ThrottlingMiddleware"/>.
    /// </summary>
    IVostokWebComponentsBuilder ConfigureThrottling(Action<IVostokThrottlingBuilder> configure);

    /// <summary>
    /// Customizes built-in <see cref="LoggingMiddleware"/>.
    /// </summary>
    IVostokWebComponentsBuilder ConfigureLogging(Action<LoggingMiddlewareOptions> configure);

    /// <summary>
    /// Customizes built-in <see cref="DatacenterAwarenessMiddleware"/>.
    /// </summary>
    IVostokWebComponentsBuilder ConfigureDatacenterAwareness(Action<DatacenterAwarenessMiddlewareOptions> configure);

    /// <summary>
    /// Customizes built-in <see cref="UnhandledExceptionMiddleware"/>.
    /// </summary>
    IVostokWebComponentsBuilder ConfigureUnhandledExceptions(Action<UnhandledExceptionMiddlewareOptions> configure);

    /// <summary>
    /// Customizes built-in <see cref="PingApiMiddleware"/>.
    /// </summary>
    IVostokWebComponentsBuilder ConfigurePingApi(Action<PingApiMiddlewareOptions> configure);

    /// <summary>
    /// Customizes built-in <see cref="DiagnosticApiMiddleware"/>.
    /// </summary>
    IVostokWebComponentsBuilder ConfigureDiagnosticApi(Action<DiagnosticApiMiddlewareOptions> configure);

    /// <summary>
    /// Customizes built-in diagnostic features (info providers and health checks).
    /// </summary>
    IVostokWebComponentsBuilder ConfigureDiagnosticFeatures(Action<DiagnosticFeaturesOptions> configure);

    /// <summary>
    /// Allows to customize <see cref="VostokMiddlewaresOptions"/>.
    /// </summary>
    internal IVostokWebComponentsBuilder Configure(Action<VostokMiddlewaresOptions> configure);
}