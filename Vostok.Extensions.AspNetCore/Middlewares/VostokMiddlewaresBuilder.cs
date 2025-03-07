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
    public IVostokMiddlewaresBuilder ConfigureHttpContextTweaks(Action<HttpContextTweakSettings> configure) =>
        Configure(configure);

    public IVostokMiddlewaresBuilder ConfigureRequestInfoFilling(Action<FillRequestInfoSettings> configure) =>
        Configure(configure);

    public IVostokMiddlewaresBuilder ConfigureDistributedContext(Action<DistributedContextSettings> configure) =>
        Configure(configure);

    public IVostokMiddlewaresBuilder ConfigureThrottling(Action<IVostokThrottlingBuilder> configure)
    {
        configure(new VostokThrottlingBuilder(services));
        return this;
    }

    public IVostokMiddlewaresBuilder ConfigureLogging(Action<LoggingSettings> configure) =>
        Configure(configure);

    public IVostokMiddlewaresBuilder ConfigureDatacenterAwareness(Action<DatacenterAwarenessSettings> configure) =>
        Configure(configure);

    public IVostokMiddlewaresBuilder ConfigureUnhandledExceptions(Action<UnhandledExceptionSettings> configure) =>
        Configure(configure);

    public IVostokMiddlewaresBuilder ConfigurePingApi(Action<PingApiSettings> configure) =>
        Configure(configure);

    public IVostokMiddlewaresBuilder ConfigureDiagnosticApi(Action<DiagnosticApiSettings> configure) =>
        Configure(configure);

    public IVostokMiddlewaresBuilder ConfigureDiagnosticFeatures(Action<DiagnosticFeaturesSettings> configure) =>
        Configure(configure);

    public IVostokMiddlewaresBuilder ConfigureOptions(Action<VostokMiddlewaresOptions> configure) =>
        Configure(configure);

    private IVostokMiddlewaresBuilder Configure<T>(Action<T> configure)
        where T : class
    {
        services.Configure(configure);
        return this;
    }
}