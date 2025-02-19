using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.Components.Diagnostics;
using Vostok.Extensions.Hosting.Components.Diagnostics.InfoProviders;
using Vostok.Hosting.Abstractions;
using Vostok.Metrics;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class DiagnosticsBuilderExtensions
{
    public static IVostokBuilder WithDiagnostics(this IVostokBuilder builder,
        Action<IVostokDiagnosticsBuilder>? configure = null)
    {
        builder.Services.AddDiagnostics();

        var diagnosticsBuilder = new VostokDiagnosticsBuilder(builder.Services);
        configure?.Invoke(diagnosticsBuilder);

        return builder;
    }

    private static void AddDiagnostics(this IServiceCollection services)
    {
        // todo (bogdanov.n) 17.02.2025: A `DiagnosticMetricsEnabled` check could take place here

        services.AddOptions<DiagnosticsInfoOptions>();

        services.TryAddSingleton<ApplicationMetricsProvider>();
        var descriptor = ServiceDescriptor.Describe(typeof(IMetricEventSender),
            sp => sp.GetRequiredService<ApplicationMetricsProvider>(), ServiceLifetime.Singleton);

        services.TryAddEnumerable(descriptor);

        services.TryAddSingleton<EnvironmentInfoProvider>();
        services.TryAddSingleton<LoadedAssembliesProvider>();
        services.TryAddSingleton<ApplicationInfoProvider>();

        services.TryAddSingleton<IVostokApplicationDiagnostics>(sp =>
        {
            var diagnosticInfo = DiagnosticInfoFactory.Create(sp);
            return new DiagnosticsHub(diagnosticInfo);
        });
    }
}