using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Helpers;

internal static class ServiceProviderExtensions
{
    public static void RegisterDisposable(this IServiceProvider serviceProvider, IDisposable disposable) =>
        serviceProvider.GetRequiredService<VostokDisposables>().Add(disposable);

    public static object? GetKeyedService(this IServiceProvider serviceProvider, Type serviceType, object serviceKey) =>
        serviceProvider switch
        {
            null => throw new ArgumentNullException(nameof(serviceProvider)),
            IKeyedServiceProvider keyedServiceProvider => keyedServiceProvider.GetKeyedService(serviceType, serviceKey),
            _ => throw new InvalidOperationException("KeyedServicesNotSupported")
        };
}