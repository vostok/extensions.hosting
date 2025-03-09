using JetBrains.Annotations;

using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Commons.Environment;
using Vostok.Extensions.AspNetCore.Diagnostics;
using Vostok.Extensions.AspNetCore.Helpers;
using Vostok.Extensions.AspNetCore.Middlewares;
using Vostok.Extensions.AspNetCore.Middlewares.PingApi;
using Vostok.Extensions.AspNetCore.Throttling;
using Vostok.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.AspNetCore;

/// <summary>
/// <para>Register Vostok middlewares and their configuration in DI container.</para>
/// <para>Use returning <see cref="IVostokMiddlewaresBuilder"/> to configure them.</para>
/// <para>Applies reasonable Kestrel defaults.</para>
/// </summary>
[PublicAPI]
public static class VostokMiddlewaresServiceCollectionExtensions
{
    /// <inheritdoc cref="VostokMiddlewaresServiceCollectionExtensions"/>
    public static IVostokMiddlewaresBuilder AddVostokMiddlewares(this IServiceCollection services)
    {
        services.TryAddSingleton<VostokDisposables>(); // todo: move to better place

        services.ConfigureKestrelDefaults();
        services.AddRequestTrackingDefaults();
        services.AddThrottlingDefaults();
        services.AddVostokHealthChecks();

        services.AddOptions<PingApiMiddlewareOptions>()
                .Configure<IServiceProvider, InitializedFlag>((settings, provider, initFlag) =>
                {
                    settings.CommitHashProvider = AssemblyCommitHashExtractor.ExtractFromEntryAssembly;
                    settings.InitializationCheck = () => initFlag.Value;

                    var diagnostics = provider.GetService<IVostokApplicationDiagnostics>();
                    if (diagnostics is not null)
                        settings.HealthCheck = () => diagnostics.HealthTracker.CurrentStatus == HealthStatus.Healthy;
                });

        return new VostokMiddlewaresBuilder(services);
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