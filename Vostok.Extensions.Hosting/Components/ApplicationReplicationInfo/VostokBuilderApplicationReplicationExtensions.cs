using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ApplicationReplicationInfo;

[PublicAPI]
public static class VostokBuilderApplicationReplicationExtensions
{
    public static IVostokBuilder WithApplicationReplicationInfo(
        this IVostokBuilder vostok,
        Action<ApplicationReplicationOptions>? configure = null)
    {
        if (!vostok.CheckComponentAlreadyAdded("ApplicationReplication"))
            vostok.Services.AddApplicationReplication();

        if (configure is not null)
            vostok.Services.Configure(configure);

        return vostok;
    }

    private static void AddApplicationReplication(this IServiceCollection services)
    {
        services.TryAddSingleton<IVostokApplicationReplicationInfo, ApplicationReplicationInfo>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<ApplicationReplicationOptions>, ApplicationReplicationOptionsValidator>());
    }
}