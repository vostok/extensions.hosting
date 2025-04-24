using JetBrains.Annotations;

using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using OpenTelemetry.Trace;

using Vostok.Commons.Environment;
using Vostok.Extensions.AspNetCore.Diagnostics;
using Vostok.Extensions.AspNetCore.Helpers;
using Vostok.Extensions.AspNetCore.HostedServices;
using Vostok.Extensions.AspNetCore.Middlewares.PingApi;
using Vostok.Extensions.AspNetCore.OpenTelemetry;
using Vostok.Extensions.AspNetCore.Throttling;
using Vostok.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.AspNetCore;

/// <summary>
/// Register Vostok web components and their configuration in DI container:
/// <list type="bullet">
///     <item>Register address from ServiceBeacon to listen and warmup ping api</item>
///     <item>Enable AspNetCore tracing instrumentation</item>
///     <item>Configure Kestrel defaults</item>
///     <item>Setup request tracking defaults</item>
///     <item>Setup throttling defaults</item>
///     <item>Setup HealthChecks</item>
///     <item>Add Vostok middlewares</item>
/// </list>
/// <para>Use returning <see cref="IVostokWebComponentsBuilder"/> to configure components.</para>
/// <para>Applies reasonable Kestrel defaults.</para>
/// </summary>
[PublicAPI]
public static class VostokWebComponentsServiceCollectionExtensions
{
    /// <inheritdoc cref="VostokWebComponentsServiceCollectionExtensions"/>
    public static IVostokWebComponentsBuilder AddVostokWebComponents(this IServiceCollection services)
    {
        services.AddHostedService<VostokAspNetCoreHostedService>();

        services.ConfigureOpenTelemetryTracerProvider(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation()
                   .SetupAspNetCoreInstrumentationForVostok();
        });

        services.ConfigureKestrelDefaults();
        services.AddRequestTrackingDefaults();
        services.AddThrottlingDefaults();
        services.AddVostokHealthChecks();

        services.AddOptions<PingApiMiddlewareOptions>()
                .Configure<IServiceProvider, IHostApplicationLifetime>((options, provider, lifetime) =>
                {
                    options.CommitHashProvider = AssemblyCommitHashExtractor.ExtractFromEntryAssembly;
                    options.InitializationCheck = () => lifetime.ApplicationStarted.IsCancellationRequested;

                    var diagnostics = provider.GetService<IVostokApplicationDiagnostics>();
                    if (diagnostics is not null)
                        options.HealthCheck = () => diagnostics.HealthTracker.CurrentStatus == HealthStatus.Healthy;
                });

        return new VostokWebComponentsBuilder(services);
    }

    private static void ConfigureKestrelDefaults(this IServiceCollection serviceCollection) =>
        serviceCollection.Configure<KestrelServerOptions>(s =>
        {
            s.AddServerHeader = false;

            s.Limits.MaxRequestBufferSize = 128 * 1024;
            s.Limits.MaxRequestLineSize = 32 * 1024;
            s.Limits.MaxRequestHeadersTotalSize = 64 * 1024;
            s.Limits.MaxConcurrentUpgradedConnections = 10000;
        });
}