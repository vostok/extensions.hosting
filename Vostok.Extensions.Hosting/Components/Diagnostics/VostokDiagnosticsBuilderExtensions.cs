using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Vostok.Extensions.Hosting.Components.Diagnostics.DiagnosticInfo.Providers;
using Vostok.Extensions.Hosting.Components.Diagnostics.HealthChecks;
using Vostok.Extensions.Hosting.Components.Diagnostics.Helpers;
using Vostok.Hosting.Abstractions.Diagnostics;
using Vostok.Metrics;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

[PublicAPI]
public static class VostokDiagnosticsBuilderExtensions
{
    public static IVostokDiagnosticsBuilder AddDiagnosticInfoProvider(
        this IVostokDiagnosticsBuilder builder,
        DiagnosticEntry entry,
        IDiagnosticInfoProvider provider) =>
        builder.AddDiagnosticInfoProvider(entry, _ => provider);

    public static IVostokDiagnosticsBuilder WithBuiltInDiagnostics(
        this IVostokDiagnosticsBuilder builder,
        Action<BuiltInDiagnosticInfoOptions>? configure = null)
    {
        builder.Services.RegisterBuiltInDiagnostics();

        builder.Services.TryAddSingleton<ApplicationMetricsProvider>();
        var metricsProviderSender =
            ServiceDescriptor.Singleton<IMetricEventSender, ApplicationMetricsProvider>(p => p.GetRequiredService<ApplicationMetricsProvider>());
        builder.Services.TryAddEnumerable(metricsProviderSender);

        if (configure is not null)
            builder.Services.Configure(configure);

        return builder;
    }

    public static IVostokDiagnosticsBuilder WithBuiltInHealthChecks(
        this IVostokDiagnosticsBuilder builder,
        Action<BuiltInHealthChecksOptions>? configure = null)
    {
        builder.Services.RegisterBuiltInHealthChecks();

        if (configure is not null)
            builder.Services.Configure(configure);

        return builder;
    }

    public static IVostokDiagnosticsBuilder AddHealthChecksMetrics(this IVostokDiagnosticsBuilder builder)
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IHealthCheckPublisher, MetricsHealthCheckPublisher>());
        return builder;
    }
}