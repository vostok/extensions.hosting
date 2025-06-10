using JetBrains.Annotations;

using Vostok.Extensions.Hosting;
using Vostok.Extensions.Hosting.Builder;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting;

[PublicAPI]
public static class VostokBuilderHostExtensions
{
    public static IVostokBuilder UseVostok(this IHostApplicationBuilder hostApplicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(hostApplicationBuilder);

        var vostok = new HostApplicationVostokBuilder(hostApplicationBuilder);
        if (!VostokBuilderCoreServices.IsAlreadyAdded(hostApplicationBuilder.Properties))
            VostokBuilderCoreServices.AddCoreServices(vostok, hostApplicationBuilder.Properties);

        return vostok;
    }

    public static IVostokBuilder UseVostok(this IHostBuilder hostBuilder)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);

        var vostok = new HostVostokBuilder(hostBuilder);
        if (!VostokBuilderCoreServices.IsAlreadyAdded(hostBuilder.Properties))
            VostokBuilderCoreServices.AddCoreServices(vostok, hostBuilder.Properties);

        return vostok;
    }
}