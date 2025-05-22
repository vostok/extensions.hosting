using JetBrains.Annotations;

using Vostok.Extensions.Hosting;
using Vostok.Extensions.Hosting.Builder;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

[PublicAPI]
public static class VostokServiceCollectionExtensions
{
    public static IVostokBuilder AddVostok(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var vostok = new VostokBuilder(services);
        vostok.AddCoreServices();

        return vostok;
    }
}