using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Vostok.Extensions.AspNetCore.Diagnostics;
using Vostok.Extensions.AspNetCore.Middlewares.Throttling;
using Vostok.Extensions.Hosting;
using Vostok.Extensions.Hosting.Components.Diagnostics.DiagnosticInfo;
using Vostok.Extensions.Hosting.Components.Diagnostics.Helpers;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Diagnostics;
using Vostok.Throttling;
using Vostok.Throttling.Config;
using Vostok.Throttling.Metrics;
using Vostok.Throttling.Quotas;

namespace Vostok.Extensions.AspNetCore.Throttling;

internal static class ThrottlingServiceCollectionExtensions
{
    public static void AddThrottlingDefaults(this IServiceCollection services)
    {
        services.AddOptions<ThrottlingConfigurationBuilder>()
                .Configure<IOptions<VostokThrottlingOptions>, IVostokApplicationLimits, ILogger<ThrottlingMiddleware>>(
                    (builder, options, limits, logger) =>
                {
                    AddThrottlingCpuLimits(limits, builder);
                    AddThrottlingErrorLogging(logger, builder);
                    AddThrottlingQuotas(options.Value, builder);
                });

        services.AddSingleton<IThrottlingProvider>(provider =>
        {
            var builder = provider.GetRequiredService<IOptions<ThrottlingConfigurationBuilder>>().Value;
            return new ThrottlingProvider(builder.Build());
        });

        services.AddSingleton<VostokWarmup.CustomWarmupAction>(AddThrottlingMetrics);

        services.AddOptions<VostokDiagnosticsOptions>()
                .Configure<IServiceProvider>(AddThrottlingInfo);

        services.AddOptions<HealthCheckServiceOptions>()
                .Configure<IServiceProvider>(AddThrottlingHealthCheck);
    }

    private static void AddThrottlingCpuLimits(
        IVostokApplicationLimits applicationLimits,
        ThrottlingConfigurationBuilder builder)
    {
        builder.SetNumberOfCores(() =>
        {
            if (applicationLimits.CpuUnits is { } cpuUnits)
                return (int)Math.Ceiling(cpuUnits);

            return Environment.ProcessorCount;
        });
    }

    private static void AddThrottlingErrorLogging(ILogger log, ThrottlingConfigurationBuilder builder)
        => builder.SetErrorCallback(ex => log.LogError(ex, "Internal failure in request throttling."));

    private static void AddThrottlingQuotas(
        VostokThrottlingOptions throttlingOptions,
        ThrottlingConfigurationBuilder builder)
    {
        if (!throttlingOptions.UseThreadPoolOverloadQuota)
            return;

        var threadPoolQuota = new ThreadPoolOverloadQuota(new ThreadPoolOverloadQuotaOptions());
        builder.AddCustomQuota(threadPoolQuota);
    }

    private static void AddThrottlingMetrics(IServiceProvider provider)
    {
        if (!provider.GetRequiredService<IOptions<VostokThrottlingOptions>>().Value.EnableMetrics)
            return;

        if (provider.GetService<IThrottlingProvider>() is not ThrottlingProvider actualProvider)
            return;

        var metrics = provider.GetService<IVostokApplicationMetrics>();
        if (metrics is null)
            return;

        var metricsOptions = provider.GetRequiredService<IOptions<ThrottlingMetricsOptions>>().Value;

        provider.RegisterDisposable(metrics.Instance.CreateThrottlingMetrics(actualProvider, metricsOptions));
    }

    private static void AddThrottlingInfo(VostokDiagnosticsOptions options, IServiceProvider provider)
    {
        if (!provider.GetRequiredService<IOptions<VostokThrottlingOptions>>().Value.AddInfoProvider)
            return;

        if (provider.GetService<IThrottlingProvider>() is not ThrottlingProvider actualProvider)
            return;

        var entry = new DiagnosticEntry(DiagnosticConstants.Component, DiagnosticConstants.RequestThrottlingEntry);
        var infoProvider = new ThrottlingInfoProvider(actualProvider);

        options.Registrations.Add(new VostokDiagnosticInfoRegistration(entry, infoProvider));
    }

    private static void AddThrottlingHealthCheck(
        HealthCheckServiceOptions healthCheckServiceOptions,
        IServiceProvider provider)
    {
        if (!provider.GetRequiredService<IOptions<VostokThrottlingOptions>>().Value.AddHealthCheck)
            return;

        if (provider.GetService<IThrottlingProvider>() is not ThrottlingProvider actualProvider)
            return;

        var check = new ThrottlingHealthCheck(actualProvider);

        healthCheckServiceOptions.Registrations.Add(
            VostokHealthCheckHelpers.RegisterWithVostokTag("Request throttling", check));
    }
}