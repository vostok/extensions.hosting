using Microsoft.Extensions.DependencyInjection;

using Vostok.Extensions.AspNetCore.Diagnostics;
using Vostok.Extensions.AspNetCore.Middlewares;
using Vostok.Extensions.AspNetCore.Middlewares.DatacentersAwareness;
using Vostok.Extensions.AspNetCore.Middlewares.DiagnosticApi;
using Vostok.Extensions.AspNetCore.Middlewares.DistributedContext;
using Vostok.Extensions.AspNetCore.Middlewares.FillRequestInfo;
using Vostok.Extensions.AspNetCore.Middlewares.HttpContextTweak;
using Vostok.Extensions.AspNetCore.Middlewares.Logging;
using Vostok.Extensions.AspNetCore.Middlewares.PingApi;
using Vostok.Extensions.AspNetCore.Middlewares.UnhandledException;
using Vostok.Extensions.AspNetCore.Throttling;

namespace Vostok.Extensions.AspNetCore;

internal sealed class VostokWebComponentsBuilder(IServiceCollection services) : IVostokWebComponentsBuilder
{
    public IVostokWebComponentsBuilder ConfigureHttpContextTweaks(Action<HttpContextTweakMiddlewareOptions> configure) =>
        Configure(configure);

    public IVostokWebComponentsBuilder ConfigureRequestInfoFilling(Action<FillRequestInfoMiddlewareOptions> configure) =>
        Configure(configure);

    public IVostokWebComponentsBuilder ConfigureDistributedContext(Action<DistributedContextMiddlewareOptions> configure) =>
        Configure(configure);

    public IVostokWebComponentsBuilder ConfigureThrottling(Action<IVostokThrottlingBuilder> configure)
    {
        configure(new VostokThrottlingBuilder(services));
        return this;
    }

    public IVostokWebComponentsBuilder ConfigureLogging(Action<LoggingMiddlewareOptions> configure) =>
        Configure(configure);

    public IVostokWebComponentsBuilder ConfigureDatacenterAwareness(Action<DatacenterAwarenessMiddlewareOptions> configure) =>
        Configure(configure);

    public IVostokWebComponentsBuilder ConfigureUnhandledExceptions(Action<UnhandledExceptionMiddlewareOptions> configure) =>
        Configure(configure);

    public IVostokWebComponentsBuilder ConfigurePingApi(Action<PingApiMiddlewareOptions> configure) =>
        Configure(configure);

    public IVostokWebComponentsBuilder ConfigureDiagnosticApi(Action<DiagnosticApiMiddlewareOptions> configure) =>
        Configure(configure);

    public IVostokWebComponentsBuilder ConfigureDiagnosticFeatures(Action<DiagnosticFeaturesOptions> configure) =>
        Configure(configure);

    public IVostokWebComponentsBuilder Configure(Action<VostokMiddlewaresOptions> configure) =>
        Configure<VostokMiddlewaresOptions>(configure);

    private IVostokWebComponentsBuilder Configure<T>(Action<T> configure)
        where T : class
    {
        services.Configure(configure);
        return this;
    }
}