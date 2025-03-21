using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Hercules.Client;
using Vostok.Hercules.Client.Abstractions;
using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Hercules;

internal static class HerculesSinkFactory
{
    public const string HerculesSinkClusterProviderKey = "HerculesSinkClusterProvider";

    public static IHerculesSink Create(IServiceProvider provider)
    {
        var settings = provider.GetRequiredService<IOptions<HerculesSinkSettings>>().Value;
        var log = provider.GetService<ILog>()?.WithMinimumLevel(LogLevel.Warn); // todo: Verbose logging?

        return new HerculesSink(settings, log);
    }
}