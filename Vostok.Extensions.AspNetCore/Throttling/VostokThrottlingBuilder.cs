using Microsoft.Extensions.DependencyInjection;

using Vostok.Extensions.AspNetCore.Middlewares.Throttling;
using Vostok.Throttling.Config;
using Vostok.Throttling.Metrics;

namespace Vostok.Extensions.AspNetCore.Throttling;

internal sealed class VostokThrottlingBuilder(IServiceCollection services) : IVostokThrottlingBuilder
{
    public IVostokThrottlingBuilder ConfigureMetrics(Action<ThrottlingMetricsOptions> configure) =>
        Configure(configure);

    public IVostokThrottlingBuilder ConfigureMiddleware(Action<ThrottlingMiddlewareOptions> configure) =>
        Configure(configure);

    public IVostokThrottlingBuilder ConfigureBuilder(Action<ThrottlingConfigurationBuilder> configure) =>
        Configure(configure);

    public IVostokThrottlingBuilder ConfigureOptions(Action<VostokThrottlingOptions> configure) =>
        Configure(configure);

    private IVostokThrottlingBuilder Configure<T>(Action<T> configure)
        where T : class
    {
        services.Configure(configure);
        return this;
    }
}