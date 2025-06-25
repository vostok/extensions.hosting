using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Vostok.Extensions.Hosting.Components.Diagnostics;
using Vostok.Extensions.Hosting.Components.Diagnostics.Helpers;
using Vostok.Extensions.Hosting.Components.Diagnostics.InfoProviders;
using Vostok.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Diagnostics;
using Vostok.Metrics;

using IHealthTracker = Vostok.Extensions.Hosting.Components.Diagnostics.IHealthTracker;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class VostokBuilderDiagnosticsExtensions
{
    public static IVostokBuilder WithDiagnostics(
        this IVostokBuilder builder,
        Action<IVostokDiagnosticsBuilder>? configure = null)
    {
        // todo (bogdanov.n) 17.02.2025: A `DiagnosticMetricsEnabled` check could take place here

        if (!builder.CheckComponentAlreadyAdded("Diagnostics"))
        {
            builder.Services.AddDiagnosticInfo();
            builder.Services.AddHealthTracker();
            builder.Services.AddDiagnosticsHub();
        }

        var diagnosticsBuilder = new VostokDiagnosticsBuilder(builder.Services);
        configure?.Invoke(diagnosticsBuilder);

        return builder;
    }

    private static void AddDiagnosticInfo(this IServiceCollection services)
    {
        services.TryAddSingleton<ApplicationMetricsProvider>();
        var metricsProviderSender = ServiceDescriptor.Singleton<IMetricEventSender, ApplicationMetricsProvider>(p =>
            p.GetRequiredService<ApplicationMetricsProvider>());
        services.TryAddEnumerable(metricsProviderSender);

        services.TryAddSingleton(DiagnosticInfoFactory.Create);
    }

    private static void AddHealthTracker(this IServiceCollection services)
    {
        services.AddHealthChecks();
        services.RegisterVostokHealthChecks();

        var tracker = new HealthTracker();
        services.TryAddSingleton<IHealthTracker>(tracker);
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHealthCheckPublisher, HealthTracker>(_ => tracker));
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHealthCheckPublisher, MetricsHealthCheckPublisher>()); // todo: Move to Kontur extensions?
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