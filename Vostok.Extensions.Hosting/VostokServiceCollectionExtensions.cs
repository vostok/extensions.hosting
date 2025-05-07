using JetBrains.Annotations;

using Vostok.Extensions.Hosting;
using Vostok.Extensions.Hosting.Builder;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

[PublicAPI]
public static class VostokServiceCollectionExtensions
{
    public static IServiceCollection AddVostok(
        this IServiceCollection services,
        Action<IVostokBuilder> configureBuilder)
    {
        ArgumentNullException.ThrowIfNull(services);

        var builder = new VostokBuilder(services);
        builder.AddCoreServices();

        configureBuilder(builder);

        return services;
    }
}