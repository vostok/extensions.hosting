using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Vostok.Extensions.Hosting.Components.Diagnostics.DiagnosticInfo;
using Vostok.Extensions.Hosting.Components.Diagnostics.DiagnosticInfo.Providers;
using Vostok.Extensions.Hosting.Components.Diagnostics.HealthChecks;
using Vostok.Extensions.Hosting.Components.Diagnostics.Helpers;
using Vostok.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Diagnostics;
using Vostok.Metrics;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

[PublicAPI]
public static class VostokBuilderDiagnosticsExtensions
{
    public static IVostokBuilder WithDiagnostics(
        this IVostokBuilder vostok,
        Action<IVostokDiagnosticsBuilder>? configure = null)
    {
        if (!vostok.CheckComponentAlreadyAdded("Diagnostics"))
        {
            vostok.Services.AddDiagnosticInfo();
            vostok.Services.AddHealthTracker();
            vostok.Services.AddDiagnosticsHub();
        }

        var diagnosticsBuilder = new VostokDiagnosticsBuilder(vostok.Services);
        configure?.Invoke(diagnosticsBuilder);

        return vostok;
    }

    private static void AddDiagnosticInfo(this IServiceCollection services)
    {
        services.TryAddSingleton<ApplicationMetricsProvider>();
        var metricsProviderSender = ServiceDescriptor.Singleton<IMetricEventSender, ApplicationMetricsProvider>(p =>
            p.GetRequiredService<ApplicationMetricsProvider>());
        services.TryAddEnumerable(metricsProviderSender);

        services.RegisterBuiltInDiagnostics();

        services.TryAddSingleton(DiagnosticInfoFactory.Create);
    }

    private static void AddHealthTracker(this IServiceCollection services)
    {
        services.AddHealthChecks();
        services.RegisterBuiltInHealthChecks();

        var tracker = new HealthTracker();
        services.TryAddSingleton<IHealthTracker>(tracker);
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHealthCheckPublisher, HealthTracker>(_ => tracker));
    }

    private static void AddDiagnosticsHub(this IServiceCollection services) =>
        services.TryAddSingleton<IVostokApplicationDiagnostics>(provider =>
        {
            var diagnosticInfo = provider.GetRequiredService<IDiagnosticInfo>();

            // note: Do not register LegacyHealthTracker in DI to prevent using it instead of new IHealthTracker with same name.
            var legacyHealthTracker = ActivatorUtilities.CreateInstance<LegacyHealthTracker>(provider);

            return new DiagnosticsHub(diagnosticInfo, legacyHealthTracker);
        });
}