using Microsoft.Extensions.DependencyInjection;

using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Logging.Internal;

internal sealed class LogConfigurator(IServiceCollection services, string? name) : ILogConfigurator
{
    public ILogConfigurator SetMinimumLevelProvider(Func<LogLevel> minLevelProvider)
    {
        Configure(options => options.MinLevelProvider = minLevelProvider);

        return this;
    }

    public ILogConfigurator CustomizeLog(Func<ILog, IServiceProvider, ILog> customize)
    {
        Configure(options => options.Customizations.Add(customize));

        return this;
    }

    private void Configure(Action<VostokLogMiscellaneousOptions> configure)
    {
        if (name is not null)
            services.Configure(name, configure);
        else
            services.Configure(configure);
    }
}