using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.AspNetCore.Diagnostics;

internal static class RequestTrackingServiceCollectionExtensions
{
    public static void AddRequestTrackingDefaults(this IServiceCollection serviceCollection) =>
        serviceCollection.TryAddSingleton<IRequestTracker>(services =>
        {
            var options = services.GetRequiredService<IOptions<DiagnosticFeaturesOptions>>().Value;

            if (!options.AddCurrentRequestsInfoProvider)
                return new DevNullRequestTracker();

            var requestTracker = new RequestTracker();
            AddRequestTrackingInfo(services, requestTracker);

            return requestTracker;
        });

    private static void AddRequestTrackingInfo(IServiceProvider services, RequestTracker requestTracker)
    {
        var diagnostics = services.GetService<IDiagnosticInfo>();
        if (diagnostics == null)
            return;

        var infoEntry = new DiagnosticEntry(DiagnosticConstants.Component, DiagnosticConstants.RequestsInProgressEntry);
        var infoProvider = new CurrentRequestsInfoProvider(requestTracker);

        services.RegisterDisposable(diagnostics.RegisterProvider(infoEntry, infoProvider));
    }
}