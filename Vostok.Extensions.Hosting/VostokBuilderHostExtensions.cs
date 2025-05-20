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

        var builder = new HostApplicationVostokBuilder(hostApplicationBuilder);
        if (!VostokBuilderCoreServices.IsAlreadyAdded(hostApplicationBuilder.Properties))
            VostokBuilderCoreServices.AddCoreServices(builder, hostApplicationBuilder.Properties);

        return builder;
    }

    public static IVostokBuilder UseVostok(this IHostBuilder hostBuilder)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);

        var builder = new HostVostokBuilder(hostBuilder);
        if (!VostokBuilderCoreServices.IsAlreadyAdded(hostBuilder.Properties))
            VostokBuilderCoreServices.AddCoreServices(builder, hostBuilder.Properties);

        return builder;
    }
}