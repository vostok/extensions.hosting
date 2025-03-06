using JetBrains.Annotations;

using Vostok.Extensions.AspNetCore.Middlewares;

namespace Vostok.Extensions.AspNetCore.Builder;

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
    /// Allows to customize <see cref="VostokMiddlewaresConfiguration"/>.
    /// </summary>
    internal IVostokMiddlewaresBuilder ConfigureOptions(Action<VostokMiddlewaresConfiguration> configure);
}