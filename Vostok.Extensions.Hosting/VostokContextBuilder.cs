using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Components.ApplicationIdentity;
using Vostok.Extensions.Hosting.Components.ApplicationReplicationInfo;
using Vostok.Hosting.Abstractions;
using Vostok.Tracing.Abstractions;
using Vostok.Tracing.Diagnostics;

namespace Vostok.Extensions.Hosting;

internal sealed class VostokContextBuilder : IVostokContextBuilder
{
    public IServiceCollection Services { get; }

    public VostokContextBuilder(IServiceCollection services)
    {
        Services = services;

        AddApplicationIdentity(services);
        AddApplicationReplicationInfo(services);
        AddTracer(services);

        services.TryAddSingleton<IVostokContext, VostokContext>();
    }

    private static void AddApplicationIdentity(IServiceCollection services)
    {
        services.AddOptionsWithValidateOnStart<ApplicationIdentityOptions>();
        services.TryAddSingleton<IValidateOptions<ApplicationIdentityOptions>, ApplicationIdentityOptionsValidator>();

        services.TryAddSingleton<IVostokApplicationIdentity, VostokApplicationIdentity>();
    }

    private static void AddApplicationReplicationInfo(IServiceCollection services) =>
        services.TryAddSingleton<Func<IVostokApplicationReplicationInfo>>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<ApplicationReplicationInfoConfigurationOptions>>().Value;

            return options.GetProvider();
        });

    private static void AddTracer(IServiceCollection services)
    {
        var activitySourceTracer = new ActivitySourceTracer(new ActivitySourceTracerSettings());
        services.TryAddSingleton<ITracer>(activitySourceTracer);
    }
}