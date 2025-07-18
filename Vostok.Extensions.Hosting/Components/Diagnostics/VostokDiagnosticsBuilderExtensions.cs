using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Vostok.Extensions.Hosting.Components.Diagnostics.DiagnosticInfo;
using Vostok.Extensions.Hosting.Components.Diagnostics.HealthChecks;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

[PublicAPI]
public static class VostokDiagnosticsBuilderExtensions
{
    public static IVostokDiagnosticsBuilder AddDiagnosticInfoProvider(
        this IVostokDiagnosticsBuilder builder,
        DiagnosticEntry entry,
        IDiagnosticInfoProvider provider) =>
        builder.AddDiagnosticInfoProvider(entry, _ => provider);

    public static IVostokDiagnosticsBuilder AddDiagnosticInfoProvider(
        this IVostokDiagnosticsBuilder builder,
        DiagnosticEntry entry,
        Func<IServiceProvider, IDiagnosticInfoProvider> implementationFactory)
    {
        var registration = new VostokDiagnosticInfoRegistration(entry, implementationFactory);
        builder.Services.Configure<VostokDiagnosticsOptions>(options => options.Registrations.Add(registration));

        return builder;
    }

    public static IVostokDiagnosticsBuilder ConfigureBuiltInDiagnostics(
        this IVostokDiagnosticsBuilder builder,
        Action<BuiltInDiagnosticInfoOptions> configure)
    {
        builder.Services.Configure(configure);
        return builder;
    }

    public static IVostokDiagnosticsBuilder ConfigureBuiltInHealthChecks(
        this IVostokDiagnosticsBuilder builder,
        Action<BuiltInHealthChecksOptions> configure)
    {
        builder.Services.Configure(configure);
        return builder;
    }

    public static IVostokDiagnosticsBuilder AddHealthChecksMetrics(this IVostokDiagnosticsBuilder builder)
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IHealthCheckPublisher, MetricsHealthCheckPublisher>());
        return builder;
    }
}