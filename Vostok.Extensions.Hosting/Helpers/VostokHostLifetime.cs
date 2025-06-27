using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Vostok.Extensions.Hosting.Helpers;

internal sealed class VostokHostLifetime(IHostLifetime baseHostLifetime, VostokWarmup warmup) : IHostLifetime
{
    public async Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        await baseHostLifetime.WaitForStartAsync(cancellationToken);
        await warmup.WarmupVostokAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken) =>
        baseHostLifetime.StopAsync(cancellationToken);
}

// note(ponomaryovigor): Decoration logic with some simplifications copied from Scrutor package.
internal static class VostokHostLifetimeDecorationHelper
{
    private const string DecoratedServiceKeySuffix = "+Decorated";

    public static void DecorateHostLifetime(this IServiceCollection services)
    {
        for (var i = services.Count - 1; i >= 0; i--)
        {
            var serviceDescriptor = services[i];

            if (serviceDescriptor.ServiceType != typeof(IHostLifetime))
                continue;

            var serviceKey = GetDecoratorKey(serviceDescriptor);

            // Insert decorated
            services.Add(serviceDescriptor.WithServiceKey(serviceKey));

            // Replace decorator
            services[i] = serviceDescriptor.WithImplementationFactory(provider =>
            {
                var baseHostLifetime = provider.GetRequiredKeyedService<IHostLifetime>(serviceKey);
                return ActivatorUtilities.CreateInstance<VostokHostLifetime>(provider, baseHostLifetime);
            });

            break;
        }
    }

    private static string GetDecoratorKey(ServiceDescriptor descriptor)
    {
        var uniqueId = Guid.NewGuid().ToString("n");
        return $"{descriptor.ServiceType.Name}+{uniqueId}{DecoratedServiceKeySuffix}";
    }

    private static ServiceDescriptor WithImplementationFactory(this ServiceDescriptor descriptor, Func<IServiceProvider, object> implementationFactory) =>
        new(descriptor.ServiceType, implementationFactory, descriptor.Lifetime);

    private static ServiceDescriptor WithServiceKey(this ServiceDescriptor descriptor, string serviceKey) =>
        descriptor switch
        {
            { ImplementationType: not null } => new ServiceDescriptor(descriptor.ServiceType, serviceKey, descriptor.ImplementationType, descriptor.Lifetime),
            { ImplementationFactory: not null } => new ServiceDescriptor(descriptor.ServiceType, serviceKey, DiscardServiceKey(descriptor.ImplementationFactory), descriptor.Lifetime),
            { ImplementationInstance: not null } => new ServiceDescriptor(descriptor.ServiceType, serviceKey, descriptor.ImplementationInstance),
            _ => throw new ArgumentException($"No implementation factory or instance or type found for {descriptor.ServiceType}.", nameof(descriptor))
        };

    private static Func<IServiceProvider, object?, object> DiscardServiceKey(Func<IServiceProvider, object> factory) =>
        (sp, _) => factory(sp);
}