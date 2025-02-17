using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Tracing.Abstractions;
using Vostok.Tracing.Diagnostics;

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

        this.WithApplicationIdentity();

        this.WithLogging();
        AddTracer(Services);
    }

    private static void AddTracer(IServiceCollection services)
    {
        var activitySourceTracer = new ActivitySourceTracer(new ActivitySourceTracerSettings());
        services.TryAddSingleton<ITracer>(activitySourceTracer);
    }

    private static void PrepareSubstitutions(IServiceCollection services)
    {
        services.AddOptionsWithValidateOnStart<SubstitutionsValidator.SubstitutionMarker>();
        services.TryAddSingleton<IValidateOptions<SubstitutionsValidator.SubstitutionMarker>, SubstitutionsValidator>();
    }
}