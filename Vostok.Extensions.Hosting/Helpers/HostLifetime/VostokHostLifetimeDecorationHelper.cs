using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Vostok.Extensions.Hosting.Helpers.HostLifetime;

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

            var baseLifetimeServiceKey = GetDecoratorKey(serviceDescriptor);

            var vostokLifetimeServiceKey = GetDecoratorKey(serviceDescriptor);
            
            // Insert vostok lifetime between
            services.Add(serviceDescriptor.WithServiceKey(vostokLifetimeServiceKey).WithImplementationFactory((provider, _) =>
            {
                var baseHostLifetime = provider.GetRequiredKeyedService<IHostLifetime>(baseLifetimeServiceKey);
                return ActivatorUtilities.CreateInstance<VostokHostLifetime>(provider, baseHostLifetime);
            }));
            
            // Insert base lifetime to the end
            services.Add(serviceDescriptor.WithServiceKey(baseLifetimeServiceKey));

            // Insert houston lifetime first
            services[i] = serviceDescriptor.WithImplementationFactory((provider, _) =>
            {
                var vostokHostLifetime = provider.GetRequiredKeyedService<IHostLifetime>(vostokLifetimeServiceKey);
                return ActivatorUtilities.CreateInstance<HoustonHostLifetime>(provider, vostokHostLifetime);
            });

            break;
        }
    }

    private static string GetDecoratorKey(ServiceDescriptor descriptor)
    {
        var uniqueId = Guid.NewGuid().ToString("n");
        return $"{descriptor.ServiceType.Name}+{uniqueId}{DecoratedServiceKeySuffix}";
    }

    private static ServiceDescriptor WithImplementationFactory(this ServiceDescriptor descriptor, Func<IServiceProvider, object?, object> implementationFactory) =>
        new(descriptor.ServiceType, descriptor.ServiceKey, implementationFactory, descriptor.Lifetime);

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