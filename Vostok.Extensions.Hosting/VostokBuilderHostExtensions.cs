using JetBrains.Annotations;

using Vostok.Extensions.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting;

[PublicAPI]
public static class VostokBuilderHostExtensions
{
    /// <summary>
    /// Configures <see cref="IHostApplicationBuilder"/> to host Vostok components.
    /// </summary>
    /// <param name="hostApplicationBuilder">The <see cref="IHostApplicationBuilder"/>.</param>
    /// <returns><see cref="IVostokBuilder"/> to configure Vostok components.</returns>
    /// <remarks>
    /// Calling this method multiple times on the same <see cref="IHostApplicationBuilder"/> instance will result in one <see cref="IVostokBuilder"/> configured.
    /// </remarks>
    public static IVostokBuilder UseVostok(this IHostApplicationBuilder hostApplicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(hostApplicationBuilder);

        var vostok = new VostokBuilder(hostApplicationBuilder.Services);
        if (!vostok.CheckAndMarkComponentAdded("CoreServices"))
            VostokBuilderCoreExtensions.AddCoreServices(vostok);

        return vostok;
    }

    /// <summary>
    /// Configures <see cref="IHostBuilder"/> to host Vostok components.
    /// </summary>
    /// <param name="hostBuilder">The <see cref="IHostBuilder"/>.</param>
    /// <param name="configure">Delegate to configure Vostok components.</param>
    /// <returns><see cref="IHostBuilder"/> for chain method calls.</returns>
    /// <remarks>
    /// Calling this method multiple times on the same <see cref="IHostBuilder"/> instance will result in one <see cref="IVostokBuilder"/> configured.
    /// </remarks>
    public static IHostBuilder UseVostok(this IHostBuilder hostBuilder, Action<IVostokBuilder> configure) =>
        hostBuilder.UseVostok((_, vostok) => configure(vostok));

    /// <inheritdoc cref="UseVostok(Microsoft.Extensions.Hosting.IHostBuilder,System.Action{Vostok.Extensions.Hosting.IVostokBuilder}?)"/>
    public static IHostBuilder UseVostok(
        this IHostBuilder hostBuilder,
        Action<HostBuilderContext, IVostokBuilder>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);

        return hostBuilder.ConfigureServices((context, services) =>
        {
            var vostok = new VostokBuilder(services);
            if (!vostok.CheckAndMarkComponentAdded("CoreServices"))
                VostokBuilderCoreExtensions.AddCoreServices(vostok);

            configure?.Invoke(context, vostok);
        });
    }
}