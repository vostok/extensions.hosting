using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Vostok.Extensions.Hosting;

[PublicAPI]
public static class HostWarmupExtensions
{
    /// <summary>
    /// Manually warmup Vostok components. Should be called right after host build.
    /// </summary>
    /// <param name="host">The <see cref="IHost"/>.</param>
    /// <returns><see cref="IHost"/> for chain method calls.</returns>
    /// <remarks>
    /// <para>You typically don't need to call it because Vostok components usually warmed up automatically by the host via custom <see cref="IHostLifetime"/> implementation.</para>
    /// <para>You can use it to manually warmup Vostok in cases:
    /// <list type="bullet">
    ///     <item>You need to overwrite <see cref="IHostLifetime"/></item>
    ///     <item>Other infrastructure strictly overwrites <see cref="IHostLifetime"/> (for example Microsoft.AspNetCore.Mvc.Testing framework)</item>
    /// </list>
    /// </para>
    /// </remarks>
    public static IHost WarmupVostok(this IHost host)
    {
        host.Services
            .GetRequiredService<VostokWarmup>()
            .WarmupVostokAsync().GetAwaiter().GetResult();

        return host;
    }
}