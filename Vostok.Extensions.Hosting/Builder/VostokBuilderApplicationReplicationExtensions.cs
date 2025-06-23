using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Components.ApplicationReplicationInfo;
using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class VostokBuilderApplicationReplicationExtensions
{
    public static IVostokBuilder WithApplicationReplicationInfo(
        this IVostokBuilder builder,
        Action<ApplicationReplicationOptions> configure)
    {
        builder.WithApplicationReplicationInfo((options, _) => configure(options));

        return builder;
    }

    public static IVostokBuilder WithApplicationReplicationInfo(
        this IVostokBuilder builder,
        Action<ApplicationReplicationOptions, IServiceProvider>? configure = null)
    {
        builder.Services.AddApplicationReplication();

        if (configure is not null)
            builder.Services.AddOptions<ApplicationReplicationOptions>().Configure(configure);

        return builder;
    }

    private static void AddApplicationReplication(this IServiceCollection services)
    {
        services.TryAddSingleton<IVostokApplicationReplicationInfo, ApplicationReplicationInfo>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<ApplicationReplicationOptions>, ApplicationReplicationOptionsValidator>());
    }
}