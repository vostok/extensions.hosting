using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

[PublicAPI]
public static class VostokDiagnosticsBuilderExtensions
{
    public static IVostokDiagnosticsBuilder ConfigureDiagnostics(
        this IVostokDiagnosticsBuilder builder,
        Action<DiagnosticsInfoOptions> configure)
    {
        builder.Services.Configure(configure);
        return builder;
    }

    public static IVostokDiagnosticsBuilder ConfigureHealthTracker(
        this IVostokDiagnosticsBuilder builder,
        Action<HealthTrackerOptions> configure)
    {
        builder.Services.Configure(configure);
        return builder;
    }
}