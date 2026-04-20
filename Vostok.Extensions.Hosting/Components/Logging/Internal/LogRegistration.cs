using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Logging.Internal;

internal sealed class LogRegistration(string logName, Func<IServiceProvider, ILog> implementationFactory)
{
    // used for tests
    internal string LogName => logName;

    private static int unnamedLogsCounter;

    public ILog Create(IServiceProvider provider)
    {
        var log = implementationFactory(provider);

        var options = provider.GetRequiredService<IOptionsMonitor<VostokLogMiscellaneousOptions>>().Get(logName);

        foreach (var customization in options.Customizations)
            log = customization(log, provider);

        if (options.MinLevelProvider is not null)
            log = log.WithMinimumLevel(options.MinLevelProvider);

        return log;
    }

    public static string GetNextName() => $"Log-{Interlocked.Increment(ref unnamedLogsCounter)}";
}