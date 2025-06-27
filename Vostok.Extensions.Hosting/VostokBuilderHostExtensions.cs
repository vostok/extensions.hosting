using JetBrains.Annotations;

using Vostok.Extensions.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

[PublicAPI]
public static class VostokBuilderHostExtensions
{
    /// <summary>
    /// Configures the service collection to host Vostok components.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns><see cref="IVostokBuilder"/> to configure components.</returns>
    /// <remarks>
    /// Calling this method multiple times on the same <see cref="IServiceCollection"/> instance will result in one <see cref="IVostokBuilder"/> configured.
    /// </remarks>
    public static IVostokBuilder AddVostok(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var vostok = new VostokBuilder(services);
        if (!vostok.CheckComponentAlreadyAdded("CoreServices"))
            VostokBuilderCoreExtensions.AddCoreServices(vostok);

        return vostok;
    }
}