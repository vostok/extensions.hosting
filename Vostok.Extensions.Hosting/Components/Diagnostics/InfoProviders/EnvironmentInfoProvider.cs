using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;

using Vostok.Commons.Environment;
using Vostok.Datacenters;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.Hosting.Components.Diagnostics.InfoProviders;

internal class EnvironmentInfoProvider(IDatacenters? datacenters) : IDiagnosticInfoProvider
{
    public object Query() => new
    {
        EnvironmentInfo.Host,
        EnvironmentInfo.FQDN,
        Datacenter = datacenters?.GetLocalDatacenter() ?? "unknown",
        EnvironmentInfo.ProcessId,
        EnvironmentInfo.ProcessName,
        EnvironmentInfo.BaseDirectory,
        Environment.CurrentDirectory,
        Environment.UserName,
        RuntimeInformation.OSDescription,
        RuntimeInformation.FrameworkDescription,
        Bitness = Environment.Is64BitProcess ? "x64" : "x86",
        Uptime = DateTime.Now - Process.GetCurrentProcess().StartTime,
        GCSettings.IsServerGC
    };
}