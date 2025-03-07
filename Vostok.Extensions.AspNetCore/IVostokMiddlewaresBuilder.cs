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
    IVostokMiddlewaresBuilder ConfigureHttpContextTweaks(Action<HttpContextTweakSettings> configure);

    /// <summary>
    /// Customizes built-in <see cref="FillRequestInfoMiddleware"/>.
    /// </summary>
    IVostokMiddlewaresBuilder ConfigureRequestInfoFilling(Action<FillRequestInfoSettings> configure);

    /// <summary>
    /// Customizes built-in <see cref="DistributedContextMiddleware"/>.
    /// </summary>
    IVostokMiddlewaresBuilder ConfigureDistributedContext(Action<DistributedContextSettings> configure);

    /// <summary>
    /// Customizes built-in <see cref="ThrottlingMiddleware"/>.
    /// </summary>
    IVostokMiddlewaresBuilder ConfigureThrottling(Action<IVostokThrottlingBuilder> configure);

    /// <summary>
    /// Customizes built-in <see cref="LoggingMiddleware"/>.
    /// </summary>
    IVostokMiddlewaresBuilder ConfigureLogging(Action<LoggingSettings> configure);

    /// <summary>
    /// Customizes built-in <see cref="DatacenterAwarenessMiddleware"/>.
    /// </summary>
    IVostokMiddlewaresBuilder ConfigureDatacenterAwareness(Action<DatacenterAwarenessSettings> configure);

    /// <summary>
    /// Customizes built-in <see cref="UnhandledExceptionMiddleware"/>.
    /// </summary>
    IVostokMiddlewaresBuilder ConfigureUnhandledExceptions(Action<UnhandledExceptionSettings> configure);

    /// <summary>
    /// Customizes built-in <see cref="PingApiMiddleware"/>.
    /// </summary>
    IVostokMiddlewaresBuilder ConfigurePingApi(Action<PingApiSettings> configure);

    /// <summary>
    /// Customizes built-in <see cref="DiagnosticApiMiddleware"/>.
    /// </summary>
    IVostokMiddlewaresBuilder ConfigureDiagnosticApi(Action<DiagnosticApiSettings> configure);

    /// <summary>
    /// Customizes built-in diagnostic features (info providers and health checks).
    /// </summary>
    IVostokMiddlewaresBuilder ConfigureDiagnosticFeatures(Action<DiagnosticFeaturesSettings> configure);

    /// <summary>
    /// Allows to customize <see cref="VostokMiddlewaresOptions"/>.
    /// </summary>
    internal IVostokMiddlewaresBuilder ConfigureOptions(Action<VostokMiddlewaresOptions> configure);
}