using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.AspNetCore.Helpers;

internal static class ServiceProviderExtensions
{
    public static void RegisterDisposable(this IServiceProvider serviceProvider, IDisposable disposable) =>
        serviceProvider.GetRequiredService<VostokDisposables>().Add(disposable);
}