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

        services.TryAddSingleton<IVostokContext, VostokContext>();
    }

    private static void AddApplicationIdentity(IServiceCollection services) =>
        services.TryAddSingleton<IVostokApplicationIdentity, VostokApplicationIdentity>();

    private static void AddApplicationReplicationInfo(IServiceCollection services) =>
        services.TryAddSingleton<Func<IVostokApplicationReplicationInfo>>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<ApplicationReplicationInfoConfigurationOptions>>().Value;

            return options.GetProvider();
        });
}