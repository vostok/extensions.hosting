using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Logging.Abstractions;
using Vostok.Tracing.Abstractions;
using Vostok.Tracing.Diagnostics;

namespace Vostok.Extensions.Hosting.Builder;

internal sealed class VostokBuilder : IVostokBuilder
{
    public IServiceCollection Services { get; }

    public VostokBuilder(IServiceCollection services)
    {
        Services = services;

        AddCoreServices();
    }

    private void AddCoreServices()
    {
        this.WithApplicationIdentity();
        this.WithApplicationReplicationInfo();
        this.WithApplicationLimits();

        AddTracer(Services);
        AddLog(Services);
    }

    private static void AddTracer(IServiceCollection services)
    {
        var activitySourceTracer = new ActivitySourceTracer(new ActivitySourceTracerSettings());
        services.TryAddSingleton<ITracer>(activitySourceTracer);
    }

    private static void AddLog(IServiceCollection services)
        => services.TryAddSingleton<ILog>(new SilentLog());
}