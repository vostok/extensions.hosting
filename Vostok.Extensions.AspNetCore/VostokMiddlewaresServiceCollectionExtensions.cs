using JetBrains.Annotations;

using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Vostok.Commons.Environment;
using Vostok.Extensions.AspNetCore.Diagnostics;
using Vostok.Extensions.AspNetCore.Helpers;
using Vostok.Extensions.AspNetCore.Middlewares;
using Vostok.Extensions.AspNetCore.Middlewares.PingApi;
using Vostok.Extensions.AspNetCore.Middlewares.Throttling;
using Vostok.Extensions.AspNetCore.Throttling;
using Vostok.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Diagnostics;
using Vostok.Throttling;
using Vostok.Throttling.Config;
using Vostok.Throttling.Metrics;
using Vostok.Throttling.Quotas;

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

        services.ConfigureKestrelDefaults()
                .AddRequestTrackingDefaults()
                .AddThrottlingDefaults()
                .AddVostokHealthChecks();

        services.AddOptions<PingApiSettings>()
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

    private static IServiceCollection ConfigureKestrelDefaults(this IServiceCollection serviceCollection)
    {
        return serviceCollection.Configure<KestrelServerOptions>(s =>
        {
            s.AddServerHeader = false;

            s.Limits.MaxRequestBufferSize = 128 * 1024;
            s.Limits.MaxRequestLineSize = 32 * 1024;
            s.Limits.MaxRequestHeadersTotalSize = 64 * 1024;
            s.Limits.MaxConcurrentUpgradedConnections = 10000;
        });
    }

    private static IServiceCollection AddRequestTrackingDefaults(this IServiceCollection serviceCollection) =>
        serviceCollection.AddSingleton<IRequestTracker>(services =>
        {
            var settings = services.GetRequiredService<IOptions<DiagnosticFeaturesSettings>>().Value;

            if (!settings.AddCurrentRequestsInfoProvider)
                return new DevNullRequestTracker();

            var requestTracker = new RequestTracker();
            AddRequestTrackingInfo(services, requestTracker);

            return requestTracker;
        });

    private static void AddRequestTrackingInfo(IServiceProvider services, RequestTracker requestTracker)
    {
        var diagnostics = services.GetService<IDiagnosticInfo>();
        if (diagnostics == null)
            return;

        var infoEntry = new DiagnosticEntry(DiagnosticConstants.Component, DiagnosticConstants.RequestsInProgressEntry);
        var infoProvider = new CurrentRequestsInfoProvider(requestTracker);

        services.RegisterDisposable(diagnostics.RegisterProvider(infoEntry, infoProvider));
    }

    private static IServiceCollection AddThrottlingDefaults(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddOptions<ThrottlingConfigurationBuilder>()
                         .Configure<IOptions<VostokThrottlingOptions>, IVostokApplicationLimits, ILogger<ThrottlingMiddleware>>(
                             (builder, options, limits, logger) =>
                             {
                                 AddThrottlingCpuLimits(limits, builder);
                                 AddThrottlingErrorLogging(logger, builder);
                                 AddThrottlingQuotas(options.Value, builder);
                             });

        serviceCollection.AddSingleton<IThrottlingProvider>(services =>
        {
            var builder = services.GetRequiredService<IOptions<ThrottlingConfigurationBuilder>>().Value;

            var provider = new ThrottlingProvider(builder.Build());
            AddThrottlingObservability(services, provider);

            return provider;
        });

        return serviceCollection;
    }

    private static void AddThrottlingCpuLimits(IVostokApplicationLimits applicationLimits, ThrottlingConfigurationBuilder builder)
    {
        builder.SetNumberOfCores(() =>
        {
            if (applicationLimits.CpuUnits is {} cpuUnits)
                return (int)Math.Ceiling(cpuUnits);

            return Environment.ProcessorCount;
        });
    }

    private static void AddThrottlingErrorLogging(ILogger log, ThrottlingConfigurationBuilder builder)
        => builder.SetErrorCallback(ex => log.LogError(ex, "Internal failure in request throttling."));

    private static void AddThrottlingQuotas(VostokThrottlingOptions throttlingOptions, ThrottlingConfigurationBuilder builder)
    {
        if (!throttlingOptions.UseThreadPoolOverloadQuota)
            return;

        var threadPoolQuota = new ThreadPoolOverloadQuota(new ThreadPoolOverloadQuotaOptions());
        builder.AddCustomQuota(threadPoolQuota);
    }

    private static void AddThrottlingObservability(IServiceProvider services, ThrottlingProvider throttlingProvider)
    {
        var diagnosticSettings = services.GetRequiredService<IOptions<DiagnosticFeaturesSettings>>().Value;

        AddThrottlingMetrics(services, throttlingProvider);

        if (diagnosticSettings.AddThrottlingInfoProvider)
            AddThrottlingInfo(services, throttlingProvider);

        if (diagnosticSettings.AddThrottlingHealthCheck)
            AddThrottlingHealthCheck(services, throttlingProvider);
    }

    private static void AddThrottlingMetrics(IServiceProvider services, ThrottlingProvider throttlingProvider)
    {
        var options = services.GetRequiredService<IOptions<VostokThrottlingOptions>>().Value;
        if (!options.EnableMetrics)
            return;

        var metrics = services.GetRequiredService<IVostokApplicationMetrics>();
        var metricsOptions = services.GetRequiredService<IOptions<ThrottlingMetricsOptions>>().Value;

        services.RegisterDisposable(metrics.Instance.CreateThrottlingMetrics(throttlingProvider, metricsOptions));
    }

    private static void AddThrottlingInfo(IServiceProvider services, ThrottlingProvider throttlingProvider)
    {
        var diagnostics = services.GetService<IDiagnosticInfo>();
        if (diagnostics == null)
            return;

        var infoEntry = new DiagnosticEntry(DiagnosticConstants.Component, DiagnosticConstants.RequestThrottlingEntry);
        var infoProvider = new ThrottlingInfoProvider(throttlingProvider);

        services.RegisterDisposable(diagnostics.RegisterProvider(infoEntry, infoProvider));
    }

    private static void AddThrottlingHealthCheck(IServiceProvider services, ThrottlingProvider throttlingProvider)
    {
        var healthTracker = services.GetService<IHealthTracker>();
        if (healthTracker == null)
            return;

        var healthCheck = new ThrottlingHealthCheck(throttlingProvider);

        services.RegisterDisposable(healthTracker.RegisterCheck("Request throttling", healthCheck));
    }
}