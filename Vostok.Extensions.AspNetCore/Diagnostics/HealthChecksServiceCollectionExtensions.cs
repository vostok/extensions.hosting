using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.AspNetCore.Diagnostics;

internal static class HealthChecksServiceCollectionExtensions
{
    public static void AddVostokHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks();

        if (services.All(service => service.ServiceType != typeof(IHealthTracker)))
            return;

        var descriptors = services.Where(service => service.ServiceType == typeof(HealthCheckService)).ToArray();
        var defaultRegistration = descriptors.First();

        var vostokRegistration = ServiceDescriptor.Singleton<HealthCheckService>(provider =>
            new VostokHealthCheckService(
                provider.GetRequiredService<IOptions<HealthCheckServiceOptions>>(),
                provider,
                provider.GetRequiredService<IHealthTracker>(),
                defaultRegistration.ImplementationType,
                provider.GetRequiredService<VostokDisposables>()));

        foreach (var descriptor in descriptors)
            services.Remove(descriptor);

        services.Add(vostokRegistration);
    }
}