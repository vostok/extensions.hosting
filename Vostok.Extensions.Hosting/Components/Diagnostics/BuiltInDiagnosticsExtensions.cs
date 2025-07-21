using JetBrains.Annotations;

namespace Vostok.Extensions.Hosting.Components.Diagnostics;

[PublicAPI]
public static class BuiltInDiagnosticsExtensions
{
    public static void DisableAll(this BuiltInDiagnosticInfoOptions options)
    {
        options.AddEnvironmentInfo = false;
        options.AddLoadedAssembliesInfo = false;
        options.AddSystemMetricsInfo = false;
        options.AddApplicationMetricsInfo = false;
        options.AddApplicationInfo = false;
        options.AddHealthChecksInfo = false;
        options.AddHerculesSinkInfo = false;
    }

    public static void DisableAll(this BuiltInHealthChecksOptions options)
    {
        options.AddDatacenterWhitelistCheck = false;
        options.AddThreadPoolStarvationCheck = false;
        options.AddZooKeeperConnectionCheck = false;
        options.AddDnsResolutionCheck = false;
        options.AddConfigurationCheck = false;
    }
}