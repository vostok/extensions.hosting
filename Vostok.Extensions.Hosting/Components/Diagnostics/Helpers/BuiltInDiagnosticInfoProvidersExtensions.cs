using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Datacenters;
using Vostok.Extensions.Hosting.Components.Diagnostics.DiagnosticInfo;
using Vostok.Extensions.Hosting.Components.Diagnostics.DiagnosticInfo.Providers;
using Vostok.Hercules.Client;
using Vostok.Hercules.Client.Abstractions;
using Vostok.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Diagnostics;
using Vostok.Metrics.System.Process;

namespace Vostok.Extensions.Hosting.Components.Diagnostics.Helpers;

internal static class BuiltInDiagnosticInfoProvidersExtensions
{
    public static void RegisterBuiltInDiagnostics(this IServiceCollection services) =>
        services.AddOptions<VostokDiagnosticsOptions>()
                .Configure<IServiceProvider>(ConfigureProviders);

    private static void ConfigureProviders(VostokDiagnosticsOptions options, IServiceProvider provider)
    {
        var registrations = options.Registrations;
        var vostokOptions = provider.GetRequiredService<IOptions<BuiltInDiagnosticInfoOptions>>().Value;

        if (vostokOptions.AddEnvironmentInfo)
        {
            var datacenters = provider.GetService<IDatacenters>();
            var infoProvider = new EnvironmentInfoProvider(datacenters);

            Add(WellKnownDiagnosticInfoProvidersNames.EnvironmentInfo, infoProvider);
        }

        var processMonitor = provider.GetService<CurrentProcessMonitor>();
        if (vostokOptions.AddSystemMetricsInfo && processMonitor is not null)
        {
            var infoProvider = new SystemMetricsProvider(processMonitor);
            Add(WellKnownDiagnosticInfoProvidersNames.SystemMetrics, infoProvider);
        }

        if (vostokOptions.AddLoadedAssembliesInfo)
        {
            Add(WellKnownDiagnosticInfoProvidersNames.LoadedAssemblies, new LoadedAssembliesProvider());
        }

        if (vostokOptions.AddHealthChecksInfo)
        {
            var healthTracker = provider.GetRequiredService<IHealthTracker>();
            var infoProvider = new HealthChecksInfoProvider(healthTracker);

            Add(WellKnownDiagnosticInfoProvidersNames.HealthChecks, infoProvider);
        }

        var herculesSink = provider.GetService<IHerculesSink>();
        if (vostokOptions.AddHerculesSinkInfo && herculesSink is HerculesSink actualSink)
        {
            Add(WellKnownDiagnosticInfoProvidersNames.HerculesSink, new HerculesSinkInfoProvider(actualSink));
        }

        if (vostokOptions.AddApplicationMetricsInfo)
        {
            var metricsProvider = provider.GetRequiredService<ApplicationMetricsProvider>();
            Add(WellKnownDiagnosticInfoProvidersNames.ApplicationMetrics, metricsProvider);
        }

        var identity = provider.GetService<IVostokApplicationIdentity>();
        var replication = provider.GetService<IVostokApplicationReplicationInfo>();
        var limits = provider.GetService<IVostokApplicationLimits>();
        if (vostokOptions.AddApplicationInfo && identity is not null && replication is not null && limits is not null)
        {
            var infoProvider = new ApplicationInfoProvider(identity, limits, replication);
            Add(WellKnownDiagnosticInfoProvidersNames.ApplicationInfo, infoProvider);
        }

        return;

        void Add(string name, IDiagnosticInfoProvider infoProvider)
        {
            var entry = new DiagnosticEntry(WellKnownDiagnosticComponentsNames.Hosting, name);
            registrations.Add(new VostokDiagnosticInfoRegistration(entry, infoProvider));
        }
    }
}