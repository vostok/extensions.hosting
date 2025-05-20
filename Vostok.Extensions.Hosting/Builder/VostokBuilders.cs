using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Extensions.Hosting.HostedServices;
using Vostok.Extensions.Hosting.OpenTelemetry;

namespace Vostok.Extensions.Hosting.Builder;

internal sealed class HostVostokBuilder(IHostBuilder hostBuilder) : IVostokBuilder
{
    public void ConfigureServices(Action<IServiceCollection> configure) =>
        hostBuilder.ConfigureServices((_, services) => configure(services));
}

internal sealed class HostApplicationVostokBuilder(IHostApplicationBuilder hostApplicationBuilder) : IVostokBuilder
{
    public void ConfigureServices(Action<IServiceCollection> configure) =>
        configure(hostApplicationBuilder.Services);
}

internal static class VostokBuilderCoreServices
{
    private const string AddedKey = "VostokCoreServicesAdded";

    public static bool IsAlreadyAdded(IDictionary<object, object> hostProperties) =>
        hostProperties.TryGetValue(AddedKey, out var value) && value is true;

    public static void AddCoreServices(IVostokBuilder vostok, IDictionary<object, object> hostProperties)
    {
        hostProperties[AddedKey] = true;

        vostok.WithApplicationIdentity();
        vostok.WithVostokLogAdapter();
        vostok.WithVostokTracerAdapter();

        vostok.ConfigureServices(services =>
        {
            services.TryAddSingleton<VostokWarmup>();
            services.DecorateHostLifetime();

            services.TryAddSingleton<VostokDisposables>();
            services.TryAddSingleton(ServiceNameProvider.Create);
        });

        vostok.ConfigureOpenTelemetryForVostok();

        VostokTypeConverterHelper.AddVostokAttributes();
    }
}