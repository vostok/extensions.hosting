using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Helpers;

internal static class ServiceProviderExtensions
{
    public static void RegisterDisposable(this IServiceProvider provider, IDisposable disposable) =>
        provider.GetRequiredService<VostokDisposables>().Add(disposable);
}