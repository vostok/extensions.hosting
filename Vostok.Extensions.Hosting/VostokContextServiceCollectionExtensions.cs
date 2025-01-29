using Vostok.Extensions.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class VostokContextServiceCollectionExtensions
{
    public static IServiceCollection AddVostok(
        this IServiceCollection services,
        Action<IVostokContextBuilder> configureBuilder)
    {
        ArgumentNullException.ThrowIfNull(services);

        var builder = new VostokContextBuilder(services);
        configureBuilder(builder);

        return services;
    }
}