using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Datacenters;
using Vostok.Extensions.Hosting.Components.Diagnostics.InfoProviders;
using Vostok.Hercules.Client;
using Vostok.Hercules.Client.Abstractions;
using Vostok.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

internal static class DiagnosticInfoFactory
{
    public static DiagnosticInfo Create(IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<DiagnosticsInfoOptions>>().Value;

        var info = new DiagnosticInfo();

        if (options.AddEnvironmentInfo)
        {
            var environmentInfoProvider = provider.GetRequiredService<EnvironmentInfoProvider>();
            var diagnosticEntry = CreateEntry(WellKnownDiagnosticInfoProvidersNames.EnvironmentInfo);
            info.RegisterProvider(diagnosticEntry, environmentInfoProvider);
        }

        if (options.AddSystemMetricsInfo)
        {
            // todo (bogdanov.n) 17.02.2025: requires `SystemMetricsProvider`
            // info.RegisterProvider(CreateEntry(WellKnownDiagnosticInfoProvidersNames.SystemMetrics), context.RegisterDisposable(new SystemMetricsProvider()));
        }

        if (options.AddLoadedAssembliesInfo)
        {
            var loadedAssembliesProvider = provider.GetRequiredService<LoadedAssembliesProvider>();
            var diagnosticEntry = CreateEntry(WellKnownDiagnosticInfoProvidersNames.LoadedAssemblies);
            info.RegisterProvider(diagnosticEntry, loadedAssembliesProvider);
        }

        if (options.AddHealthChecksInfo)
        {
            // todo (bogdanov.n) 14.02.2025: requires `IHealthTracker`
            // info.RegisterProvider(CreateEntry(WellKnownDiagnosticInfoProvidersNames.HealthChecks), new HealthChecksInfoProvider(healthTracker));
        }

        if (options.AddConfigurationInfo)
        {
            // todo (bogdanov.n) 14.02.2025: requires `IConfigurationSource`

            // note (bogdanov.n) 14.02.2025: configuration can contain secrets,
            // be careful about what IConfigurationSource you passing

            // info.RegisterProvider(CreateEntry(WellKnownDiagnosticInfoProvidersNames.Configuration), new ConfigurationInfoProvider());
        }

        var herculesSink = provider.GetRequiredService<IHerculesSink>();
        if (options.AddHerculesSinkInfo && herculesSink is HerculesSink realSink)
        {
            var herculesSinkInfoProvider = new HerculesSinkInfoProvider(realSink);
            var diagnosticEntry = CreateEntry(WellKnownDiagnosticInfoProvidersNames.HerculesSink);
            info.RegisterProvider(diagnosticEntry, herculesSinkInfoProvider);
        }

        if (options.AddApplicationMetricsInfo)
        {
            var metricsProvider = provider.GetRequiredService<ApplicationMetricsProvider>();
            var diagnosticEntry = CreateEntry(WellKnownDiagnosticInfoProvidersNames.ApplicationMetrics);
            info.RegisterProvider(diagnosticEntry, metricsProvider);
        }

        if (options.AddApplicationInfo)
        {
            var applicationInfoProvider = provider.GetRequiredService<ApplicationInfoProvider>();
            var diagnosticEntry = CreateEntry(WellKnownDiagnosticInfoProvidersNames.ApplicationInfo);
            info.RegisterProvider(diagnosticEntry, applicationInfoProvider);
        }

        return info;
    }

    private static DiagnosticEntry CreateEntry(string name) => new(WellKnownDiagnosticComponentsNames.Hosting, name);
}