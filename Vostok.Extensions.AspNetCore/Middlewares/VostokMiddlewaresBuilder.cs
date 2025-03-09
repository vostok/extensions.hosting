using Microsoft.Extensions.DependencyInjection;

using Vostok.Extensions.AspNetCore.Diagnostics;
using Vostok.Extensions.AspNetCore.Middlewares.DatacentersAwareness;
using Vostok.Extensions.AspNetCore.Middlewares.DiagnosticApi;
using Vostok.Extensions.AspNetCore.Middlewares.DistributedContext;
using Vostok.Extensions.AspNetCore.Middlewares.FillRequestInfo;
using Vostok.Extensions.AspNetCore.Middlewares.HttpContextTweak;
using Vostok.Extensions.AspNetCore.Middlewares.Logging;
using Vostok.Extensions.AspNetCore.Middlewares.PingApi;
using Vostok.Extensions.AspNetCore.Middlewares.UnhandledException;
using Vostok.Extensions.AspNetCore.Throttling;

namespace Vostok.Extensions.AspNetCore.Middlewares;

internal sealed class VostokMiddlewaresBuilder(IServiceCollection services) : IVostokMiddlewaresBuilder
{
    public IVostokMiddlewaresBuilder ConfigureHttpContextTweaks(Action<HttpContextTweakMiddlewareOptions> configure) =>
        Configure(configure);

    public IVostokMiddlewaresBuilder ConfigureRequestInfoFilling(Action<FillRequestInfoMiddlewareOptions> configure) =>
        Configure(configure);

    public IVostokMiddlewaresBuilder ConfigureDistributedContext(Action<DistributedContextMiddlewareOptions> configure) =>
        Configure(configure);

    public IVostokMiddlewaresBuilder ConfigureThrottling(Action<IVostokThrottlingBuilder> configure)
    {
        configure(new VostokThrottlingBuilder(services));
        return this;
    }

    public IVostokMiddlewaresBuilder ConfigureLogging(Action<LoggingMiddlewareOptions> configure) =>
        Configure(configure);

    public IVostokMiddlewaresBuilder ConfigureDatacenterAwareness(Action<DatacenterAwarenessMiddlewareOptions> configure) =>
        Configure(configure);

    public IVostokMiddlewaresBuilder ConfigureUnhandledExceptions(Action<UnhandledExceptionMiddlewareOptions> configure) =>
        Configure(configure);

    public IVostokMiddlewaresBuilder ConfigurePingApi(Action<PingApiMiddlewareOptions> configure) =>
        Configure(configure);

    public IVostokMiddlewaresBuilder ConfigureDiagnosticApi(Action<DiagnosticApiMiddlewareOptions> configure) =>
        Configure(configure);

    public IVostokMiddlewaresBuilder ConfigureDiagnosticFeatures(Action<DiagnosticFeaturesOptions> configure) =>
        Configure(configure);

    public IVostokMiddlewaresBuilder Configure(Action<VostokMiddlewaresOptions> configure) =>
        Configure<VostokMiddlewaresOptions>(configure);

    private IVostokMiddlewaresBuilder Configure<T>(Action<T> configure)
        where T : class
    {
        services.Configure(configure);
        return this;
    }
}