using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.AspNetCore.Diagnostics;

internal static class RequestTrackingServiceCollectionExtensions
{
    public static void AddRequestTrackingDefaults(this IServiceCollection services) =>
        services.TryAddSingleton<IRequestTracker>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<DiagnosticFeaturesOptions>>().Value;

            if (!options.AddCurrentRequestsInfoProvider)
                return new DevNullRequestTracker();

            var requestTracker = new RequestTracker();
            AddRequestTrackingInfo(provider, requestTracker);

            return requestTracker;
        });

    private static void AddRequestTrackingInfo(IServiceProvider provider, RequestTracker requestTracker)
    {
        var diagnostics = provider.GetService<IDiagnosticInfo>();
        if (diagnostics == null)
            return;

        var infoEntry = new DiagnosticEntry(DiagnosticConstants.Component, DiagnosticConstants.RequestsInProgressEntry);
        var infoProvider = new CurrentRequestsInfoProvider(requestTracker);

        provider.RegisterDisposable(diagnostics.RegisterProvider(infoEntry, infoProvider));
    }
}