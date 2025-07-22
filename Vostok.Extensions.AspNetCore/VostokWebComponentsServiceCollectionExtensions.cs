using JetBrains.Annotations;

using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

using OpenTelemetry.Trace;

using Vostok.Commons.Environment;
using Vostok.Extensions.AspNetCore.Diagnostics;
using Vostok.Extensions.AspNetCore.HostedServices;
using Vostok.Extensions.AspNetCore.Middlewares.PingApi;
using Vostok.Extensions.AspNetCore.OpenTelemetry;
using Vostok.Extensions.AspNetCore.Throttling;
using Vostok.Extensions.Hosting.Components.Diagnostics;

namespace Vostok.Extensions.AspNetCore;

/// <summary>
/// Register Vostok web components and their configuration in DI container:
/// <list type="bullet">
///     <item>Register address from ServiceBeacon to listen and warmup ping api</item>
///     <item>Register Vostok middlewares</item>
///     <item>Enable AspNetCore tracing instrumentation</item>
///     <item>Enable HealthChecks</item>
///     <item>Setup reasonable Kestrel defaults</item>
///     <item>Setup request tracking defaults</item>
///     <item>Setup throttling defaults</item>
/// </list>
/// <para>Use returning <see cref="IVostokWebComponentsBuilder"/> to configure components.</para>
/// </summary>
[PublicAPI]
public static class VostokWebComponentsServiceCollectionExtensions
{
    /// <inheritdoc cref="VostokWebComponentsServiceCollectionExtensions"/>
    public static IVostokWebComponentsBuilder AddVostokWebComponents(this IServiceCollection services)
    {
        // todo: idempotency
        services.ConfigureKestrelDefaults();
        services.ConfigureAspNetCoreTelemetry();
        services.AddRequestTrackingDefaults();
        services.AddThrottlingDefaults();

        services.AddHealthChecks();

        services.AddHostedService<VostokAspNetCoreHostedService>();

        services.AddOptions<PingApiMiddlewareOptions>()
                .Configure<IServiceProvider, IHostApplicationLifetime>((options, provider, lifetime) =>
                {
                    options.CommitHashProvider = AssemblyCommitHashExtractor.ExtractFromEntryAssembly;
                    options.InitializationCheck = () => lifetime.ApplicationStarted.IsCancellationRequested;

                    var healthTracker = provider.GetService<IHealthTracker>();
                    if (healthTracker is not null)
                        options.HealthCheck = () => healthTracker.CurrentReport.Status is HealthStatus.Healthy;
                });

        return new VostokWebComponentsBuilder(services);
    }

    private static void ConfigureKestrelDefaults(this IServiceCollection services) =>
        services.Configure<KestrelServerOptions>(options =>
        {
            options.AddServerHeader = false;

            options.Limits.MaxRequestBufferSize = 128 * 1024;
            options.Limits.MaxResponseBufferSize = 64 * 1024;

            options.Limits.MaxRequestBodySize = null;
            options.Limits.MaxRequestLineSize = 32 * 1024;
            options.Limits.MaxRequestHeadersTotalSize = 64 * 1024;
            options.Limits.MaxConcurrentUpgradedConnections = 10000;
        });

    private static void ConfigureAspNetCoreTelemetry(this IServiceCollection services)
    {
        services.ConfigureOpenTelemetryTracerProvider(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation()
                   .SetupAspNetCoreInstrumentationForVostok();
        });
    }
}