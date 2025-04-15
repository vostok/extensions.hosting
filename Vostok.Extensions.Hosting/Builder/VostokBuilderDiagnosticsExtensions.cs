using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.Components.Diagnostics;
using Vostok.Extensions.Hosting.Components.Diagnostics.InfoProviders;
using Vostok.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Diagnostics;
using Vostok.Metrics;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class VostokBuilderDiagnosticsExtensions
{
    public static IVostokBuilder WithDiagnostics(this IVostokBuilder builder,
        Action<IVostokDiagnosticsBuilder>? configure = null)
    {
        // todo (bogdanov.n) 17.02.2025: A `DiagnosticMetricsEnabled` check could take place here

        builder.Services.AddDiagnostics();
        builder.Services.AddHealthTracker();
        builder.Services.TryAddSingleton<IVostokApplicationDiagnostics, DiagnosticsHub>();

        var diagnosticsBuilder = new VostokDiagnosticsBuilder(builder.Services);
        configure?.Invoke(diagnosticsBuilder);

        return builder;
    }

    private static void AddDiagnostics(this IServiceCollection services)
    {
        services.TryAddSingleton<ApplicationMetricsProvider>();
        var metricsProviderSender = ServiceDescriptor.Singleton<IMetricEventSender, ApplicationMetricsProvider>(p =>
            p.GetRequiredService<ApplicationMetricsProvider>());
        services.TryAddEnumerable(metricsProviderSender);

        services.TryAddSingleton<IDiagnosticInfo>(DiagnosticInfoFactory.Create);
    }

    private static void AddHealthTracker(this IServiceCollection services) =>
        services.TryAddSingleton<IHealthTracker>(HealthTrackerFactory.Create);
}