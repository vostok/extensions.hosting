using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vostok.Applications.Scheduled;
using Vostok.Hosting.Abstractions;
using Vostok.Logging.Abstractions;
using Vostok.Tracing.Abstractions;

namespace Vostok.Extensions.Scheduled;

[PublicAPI]
public static class ServiceCollectionScheduledExtensions
{
    public static IServiceCollection AddScheduledService<THostedService>(this IServiceCollection services)
        where THostedService : ScheduledService
    {
        services.AddScheduledActionsBuilder();
        services.AddHostedService<THostedService>();

        return services;
    }

    public static IServiceCollection AddScheduledService<THostedService>(
        this IServiceCollection services,
        Func<IServiceProvider, THostedService> implementationFactory)
        where THostedService : ScheduledService
    {
        services.AddScheduledActionsBuilder();
        services.AddHostedService(implementationFactory);

        return services;
    }

    private static void AddScheduledActionsBuilder(this IServiceCollection services) =>
        services.TryAddTransient<IScheduledActionsBuilder>(provider =>
            new ScheduledActionsBuilder(
                provider.GetRequiredService<ILog>(),
                provider.GetRequiredService<ITracer>(),
                provider.GetService<IVostokApplicationDiagnostics>()));
}