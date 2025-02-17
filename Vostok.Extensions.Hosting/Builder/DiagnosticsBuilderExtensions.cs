using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.Components.Diagnostics;
using Vostok.Extensions.Hosting.Components.Diagnostics.InfoProviders;
using Vostok.Hosting.Abstractions;
using Vostok.Metrics;

namespace Vostok.Extensions.Hosting.Builder;

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

        services.AddOptionsWithValidateOnStart<DiagnosticsInfoOptions>();

        var metricsProvider = new ApplicationMetricsProvider();
        services.TryAddSingleton(metricsProvider);
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IMetricEventSender, ApplicationMetricsProvider>());

        services.TryAddSingleton<IVostokApplicationDiagnostics>(sp =>
        {
            var diagnosticInfo = DiagnosticInfoFactory.Create(sp);
            return new DiagnosticsHub(diagnosticInfo);
        });
    }
}