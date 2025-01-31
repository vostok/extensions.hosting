using Vostok.Extensions.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class VostokServiceCollectionExtensions
{
    public static IServiceCollection AddVostok(
        this IServiceCollection services,
        Action<IVostokBuilder> configureBuilder)
    {
        ArgumentNullException.ThrowIfNull(services);

        var builder = new VostokBuilder(services);
        configureBuilder(builder);

        return services;
    }
}