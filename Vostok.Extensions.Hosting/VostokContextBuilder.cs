using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Components.ApplicationIdentity;
using Vostok.Extensions.Hosting.Components.ApplicationReplicationInfo;
using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting;

internal sealed class VostokContextBuilder : IVostokContextBuilder
{
    public IServiceCollection Services { get; }

    public VostokContextBuilder(IServiceCollection services)
    {
        Services = services;

        AddApplicationIdentity(services);
        AddApplicationReplicationInfo(services);
    }

    private static void AddApplicationIdentity(IServiceCollection services) =>
        services.TryAddSingleton<IVostokApplicationIdentity>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<VostokApplicationIdentityConfigurationOptions>>().Value;

            return new VostokApplicationIdentity(
                options.Project,
                options.Subproject,
                options.Environment,
                options.Application,
                options.Instance);
        });

    private static void AddApplicationReplicationInfo(IServiceCollection services) =>
        services.TryAddSingleton<Func<IVostokApplicationReplicationInfo>>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<ApplicationReplicationInfoConfigurationOptions>>().Value;

            return options.GetProvider();
        });
}