using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Components.ApplicationReplicationInfo;
using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class ApplicationReplicationBuilderExtensions
{
    public static IVostokBuilder WithApplicationReplicationInfo(
        this IVostokBuilder builder,
        Action<ApplicationReplicationInfoConfigurationOptions> configure)
    {
        builder.WithApplicationReplicationInfo((options, _) => configure(options));

        return builder;
    }

    public static IVostokBuilder WithApplicationReplicationInfo(
        this IVostokBuilder builder,
        Action<ApplicationReplicationInfoConfigurationOptions, IServiceProvider>? configure = null)
    {
        builder.Services.AddApplicationReplication();

        if (configure is not null)
            builder.Services.AddOptions<ApplicationReplicationInfoConfigurationOptions>().Configure(configure);

        return builder;
    }

    private static void AddApplicationReplication(this IServiceCollection services) =>
        services.TryAddSingleton<Func<IVostokApplicationReplicationInfo>>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<ApplicationReplicationInfoConfigurationOptions>>().Value;

            return options.GetProvider();
        });
}