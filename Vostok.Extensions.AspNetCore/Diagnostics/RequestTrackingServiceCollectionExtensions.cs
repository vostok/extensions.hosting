using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Components.Diagnostics.DiagnosticInfo;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.AspNetCore.Diagnostics;

internal static class RequestTrackingServiceCollectionExtensions
{
    public static void AddRequestTrackingDefaults(this IServiceCollection services)
    {
        services.TryAddSingleton<IRequestTracker>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<DiagnosticFeaturesOptions>>().Value;

            return options.AddCurrentRequestsInfoProvider
                ? new RequestTracker()
                : new DevNullRequestTracker();
        });

        services.AddOptions<VostokDiagnosticsOptions>()
                .Configure<IRequestTracker>(AddRequestTrackingInfo);
    }

    private static void AddRequestTrackingInfo(VostokDiagnosticsOptions options, IRequestTracker tracker)
    {
        if (tracker is not RequestTracker actualTracker)
            return;

        var entry = new DiagnosticEntry(DiagnosticConstants.Component, DiagnosticConstants.RequestsInProgressEntry);
        var infoProvider = new CurrentRequestsInfoProvider(actualTracker);

        options.Registrations.Add(new VostokDiagnosticInfoRegistration(entry, infoProvider));
    }
}