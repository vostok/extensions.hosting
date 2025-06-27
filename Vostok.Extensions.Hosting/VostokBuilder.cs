using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.Components;
using Vostok.Extensions.Hosting.Components.ApplicationIdentity;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.Extensions.Hosting.Helpers.TypeConverters;
using Vostok.Extensions.Hosting.OpenTelemetry;

namespace Vostok.Extensions.Hosting;

internal sealed class VostokBuilder(IServiceCollection services) : IVostokBuilder
{
    public IServiceCollection Services { get; } = services;
}

internal static class VostokBuilderCoreExtensions
{
    public static void AddCoreServices(IVostokBuilder vostok)
    {
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

    public static bool CheckComponentAlreadyAdded(this IVostokBuilder vostok, string componentName)
    {
        if (vostok.Services
                  .SingleOrDefault(sd => sd.ServiceType == typeof(RegisteredVostokComponents))?
                  .ImplementationInstance is not RegisteredVostokComponents registeredComponents)
        {
            registeredComponents = new RegisteredVostokComponents([]);
            vostok.Services.TryAddSingleton(registeredComponents);
        }

        return !registeredComponents.Components.Add(componentName);
    }

    private sealed record RegisteredVostokComponents(HashSet<string> Components);
}