using Microsoft.Extensions.DependencyInjection;

using Vostok.Clusterclient.Core.Topology;
using Vostok.Hercules.Client;

namespace Vostok.Extensions.Hosting.Components.Hercules;

internal sealed class HerculesSinkBuilder(IServiceCollection services) : IHerculesSinkBuilder
{
    public IServiceCollection Services { get; } = services;

    public IHerculesSinkBuilder Configure(Action<HerculesSinkSettings> configure)
    {
        Services.Configure(configure);
        return this;
    }

    public IHerculesSinkBuilder SetApiKey(string? apiKey)
    {
        Services.Configure<HerculesSinkAuthOptions>(o => o.ApiKey = apiKey);
        return this;
    }

    public IHerculesSinkBuilder SetClusterProvider(Func<IServiceProvider, IClusterProvider?> implementationFactory)
    {
        Services.Configure<HerculesSinkMiscellaneousOptions>(options => options.ClusterProvider = implementationFactory);
        return this;
    }
}