using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Extensions.Hosting.HostedServices;
using Vostok.Extensions.Hosting.OpenTelemetry;

namespace Vostok.Extensions.Hosting.Builder;

internal sealed class VostokBuilder(IServiceCollection services) : IVostokBuilder
{
    public IServiceCollection Services { get; } = services;
}

internal static class VostokBuilderCoreServices
{
    private const string AddedKey = "_VostokCoreServicesAdded";

    public static bool IsAlreadyAdded(IDictionary<object, object> hostProperties) =>
        hostProperties.TryGetValue(AddedKey, out var value) && value is true;

    public static void AddCoreServices(IVostokBuilder vostok, IDictionary<object, object> hostProperties)
    {
        hostProperties[AddedKey] = true;

        vostok.Services.TryAddSingleton<VostokWarmup>();
        vostok.Services.DecorateHostLifetime();

        vostok.WithApplicationIdentity();
        vostok.WithVostokLogAdapter();
        vostok.WithVostokTracerAdapter();

        vostok.Services.TryAddSingleton<VostokDisposables>();
        vostok.Services.TryAddSingleton(ServiceNameProvider.Create);

        vostok.ConfigureOpenTelemetryForVostok();

        VostokTypeConverterHelper.AddVostokAttributes();
    }
}