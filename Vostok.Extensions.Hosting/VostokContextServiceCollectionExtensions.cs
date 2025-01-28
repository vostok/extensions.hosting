using Vostok.Extensions.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class VostokContextServiceCollectionExtensions
{
    public static IVostokContextBuilder AddVostokContext(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return new VostokContextBuilder(services);
    }
}