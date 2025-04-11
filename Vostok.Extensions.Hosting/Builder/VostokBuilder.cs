using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Extensions.Hosting.HostedServices;
using Vostok.Extensions.Hosting.OpenTelemetry;

namespace Vostok.Extensions.Hosting.Builder;

internal sealed class VostokBuilder(IServiceCollection services) : IVostokBuilder
{
    public IServiceCollection Services { get; } = services;

    public void AddCoreServices()
    {
        Services.Decorate<IHostLifetime, VostokHostLifetime>();
        Services.TryAddSingleton<VostokStartupService>();

        PrepareSubstitutions(Services);

        this.WithApplicationIdentity();
        this.WithVostokLogAdapter();
        this.WithVostokTracerAdapter();

        this.ConfigureOpenTelemetryForVostok();

        Services.TryAddSingleton<VostokDisposables>();
        Services.TryAddSingleton(ServiceNameProvider.Create);

        VostokTypeConverterHelper.AddVostokAttributes();
    }

    private static void PrepareSubstitutions(IServiceCollection services)
    {
        services.AddOptionsWithValidateOnStart<SubstitutionsValidator.SubstitutionMarker>();
        services.TryAddSingleton<IValidateOptions<SubstitutionsValidator.SubstitutionMarker>, SubstitutionsValidator>();
    }
}