using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Hercules.Client;
using Vostok.Hercules.Client.Abstractions;
using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Hercules;

internal static class HerculesSinkFactory
{
    public static IHerculesSink Create(IServiceProvider provider)
    {
        try
        {
            var settings = provider.GetRequiredService<IOptions<HerculesSinkSettings>>().Value;
            var log = provider.GetInitializationLog();

            if (settings.SuppressVerboseLogging)
                log = log.WithMinimumLevel(LogLevel.Warn);

            return new HerculesSink(settings, log);
        }
        catch (Exception e)
        {
            provider.GetInitializationLog().LogDisabled("HerculesSink", e.Message);
            return new DevNullHerculesSink();
        }
    }

    public static IHerculesSink CreateDisabled(IServiceProvider provider)
    {
        provider.GetInitializationLog().LogDisabled("HerculesSink");
        return new DevNullHerculesSink();
    }
}