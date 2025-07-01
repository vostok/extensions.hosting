using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Logging.Console;

namespace Vostok.Extensions.Hosting.Components.Logging.Console;

internal static class ConsoleLogFactory
{
    public const string LogName = "Console";

    public static ConsoleLog Create(IServiceProvider provider)
    {
        var settings = provider.GetRequiredService<IOptions<ConsoleLogSettings>>().Value;
        return new ConsoleLog(settings);
    }
}