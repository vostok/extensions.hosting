using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Extensions.Hosting.HostedServices;
using Vostok.Extensions.Hosting.OpenTelemetry;

namespace Vostok.Extensions.Hosting.Builder;

internal sealed class VostokBuilder(IServiceCollection services) : IVostokBuilder
{
    public IServiceCollection Services { get; } = services;

    public void AddCoreServices()
    {
        Services.TryAddSingleton<VostokWarmup>();
        Services.DecorateHostLifetime();

        this.WithApplicationIdentity();
        this.WithVostokLogAdapter();
        this.WithVostokTracerAdapter();

        this.ConfigureOpenTelemetryForVostok();

        Services.TryAddSingleton<VostokDisposables>();
        Services.TryAddSingleton(ServiceNameProvider.Create);

        VostokTypeConverterHelper.AddVostokAttributes();
    }
}