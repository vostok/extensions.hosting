using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Datacenters;
using Vostok.Extensions.Hosting.Components.Diagnostics.DiagnosticInfo.Providers;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.Hercules.Client;
using Vostok.Hercules.Client.Abstractions;
using Vostok.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Diagnostics;
using Vostok.Metrics.System.Process;

namespace Vostok.Extensions.Hosting.Components.Diagnostics.DiagnosticInfo;

internal static class DiagnosticInfoFactory
{
    public static IDiagnosticInfo Create(IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<DiagnosticsInfoOptions>>().Value;

        var info = new VostokDiagnosticInfo();

        if (options.AddEnvironmentInfo)
        {
            var datacenters = provider.GetService<IDatacenters>();
            var diagnosticEntry = CreateEntry(WellKnownDiagnosticInfoProvidersNames.EnvironmentInfo);
            provider.RegisterDisposable(info.RegisterProvider(diagnosticEntry, new EnvironmentInfoProvider(datacenters)));
        }

        var processMonitor = provider.GetService<CurrentProcessMonitor>();
        if (options.AddSystemMetricsInfo && processMonitor is not null)
        {
            var systemMetricsEntry = CreateEntry(WellKnownDiagnosticInfoProvidersNames.SystemMetrics);
            var systemMetricsProvider = new SystemMetricsProvider(processMonitor);
            info.RegisterProvider(systemMetricsEntry, systemMetricsProvider);

            provider.RegisterDisposable(systemMetricsProvider);
        }

        if (options.AddLoadedAssembliesInfo)
        {
            var diagnosticEntry = CreateEntry(WellKnownDiagnosticInfoProvidersNames.LoadedAssemblies);
            provider.RegisterDisposable(info.RegisterProvider(diagnosticEntry, new LoadedAssembliesProvider()));
        }

        if (options.AddHealthChecksInfo)
        {
            var healthTracker = provider.GetRequiredService<IHealthTracker>();
            var healthChecksEntry = CreateEntry(WellKnownDiagnosticInfoProvidersNames.HealthChecks);
            provider.RegisterDisposable(info.RegisterProvider(healthChecksEntry, new HealthChecksInfoProvider(healthTracker)));
        }

        var herculesSink = provider.GetService<IHerculesSink>();
        if (options.AddHerculesSinkInfo && herculesSink is HerculesSink realSink)
        {
            var diagnosticEntry = CreateEntry(WellKnownDiagnosticInfoProvidersNames.HerculesSink);
            provider.RegisterDisposable(info.RegisterProvider(diagnosticEntry, new HerculesSinkInfoProvider(realSink)));
        }

        if (options.AddApplicationMetricsInfo)
        {
            var metricsProvider = provider.GetRequiredService<ApplicationMetricsProvider>();
            var diagnosticEntry = CreateEntry(WellKnownDiagnosticInfoProvidersNames.ApplicationMetrics);
            provider.RegisterDisposable(info.RegisterProvider(diagnosticEntry, metricsProvider));
        }

        var identity = provider.GetService<IVostokApplicationIdentity>();
        var replication = provider.GetService<IVostokApplicationReplicationInfo>();
        var limits = provider.GetService<IVostokApplicationLimits>();
        if (options.AddApplicationInfo && identity is not null && replication is not null && limits is not null)
        {
            var applicationInfoProvider = new ApplicationInfoProvider(identity, limits, replication);
            var diagnosticEntry = CreateEntry(WellKnownDiagnosticInfoProvidersNames.ApplicationInfo);
            provider.RegisterDisposable(info.RegisterProvider(diagnosticEntry, applicationInfoProvider));
        }

        return info;
    }

    private static DiagnosticEntry CreateEntry(string name) => new(WellKnownDiagnosticComponentsNames.Hosting, name);
}