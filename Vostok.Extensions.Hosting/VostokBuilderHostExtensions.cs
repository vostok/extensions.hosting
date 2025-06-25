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

        var vostok = new VostokBuilder(hostApplicationBuilder.Services);
        if (!vostok.CheckComponentAlreadyAdded("CoreServices"))
            VostokBuilderExtensions.AddCoreServices(vostok);

        return vostok;
    }

    public static IHostBuilder UseVostok(
        this IHostBuilder hostBuilder,
        Action<IVostokBuilder>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);

        hostBuilder.UseVostok((_, builder) => configure?.Invoke(builder));

        return hostBuilder;
    }

    public static IHostBuilder UseVostok(
        this IHostBuilder hostBuilder,
        Action<HostBuilderContext, IVostokBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);

        hostBuilder.ConfigureServices((context, services) =>
        {
            var vostok = new VostokBuilder(services);
            if (!vostok.CheckComponentAlreadyAdded("CoreServices"))
                VostokBuilderExtensions.AddCoreServices(vostok);

            configure.Invoke(context, vostok);
        });

        return hostBuilder;
    }
}