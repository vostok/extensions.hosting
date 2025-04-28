using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Extensions.Hosting.HostedServices;
using Vostok.Extensions.Hosting.OpenTelemetry;

namespace Vostok.Extensions.Hosting.Builder;

internal sealed class VostokBuilder : IVostokBuilder
{
    public IServiceCollection Services { get; }

    public VostokBuilder(IServiceCollection services)
    {
        Services = services;

        AddCoreServices();
    }

    private void AddCoreServices()
    {
        PrepareSubstitutions(Services);

        Services.TryAddSingleton<VostokDisposables>();
        Services.AddHostedService<VostokStartupService>();

        this.WithApplicationIdentity();

        this.EnableOuterVostokLog();
        this.WithTracing();
        
        Services.TryAddSingleton(ServiceNameProvider.Create);

        this.ConfigureOpenTelemetryForVostok();
    }

    private static void PrepareSubstitutions(IServiceCollection services)
    {
        services.AddOptionsWithValidateOnStart<SubstitutionsValidator.SubstitutionMarker>();
        services.TryAddSingleton<IValidateOptions<SubstitutionsValidator.SubstitutionMarker>, SubstitutionsValidator>();
    }
}