using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.Hosting.Components.Diagnostics.DiagnosticInfo;

internal static class DiagnosticInfoFactory
{
    public static IDiagnosticInfo Create(IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<VostokDiagnosticsOptions>>().Value;

        var info = new VostokDiagnosticInfo();

        foreach (var registration in options.Registrations)
        {
            var infoProvider = registration.ImplementationFactory(provider);
            provider.RegisterDisposable(info.RegisterProvider(registration.Entry, infoProvider));
        }

        return info;
    }

}