using JetBrains.Annotations;

using Vostok.Extensions.Hosting;
using Vostok.Extensions.Hosting.Builder;
using Vostok.Extensions.Hosting.Helpers;

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

        VostokTypeConverterHelper.AddVostokAttributes();

        var builder = new VostokBuilder(services);
        configureBuilder(builder);

        return services;
    }
}