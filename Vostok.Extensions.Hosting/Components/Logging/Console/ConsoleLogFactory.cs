using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Commons.Helpers.Disposable;
using Vostok.Extensions.Hosting.Components.Logging.Builder;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Console;

namespace Vostok.Extensions.Hosting.Components.Logging.Console;

internal static class ConsoleLogFactory
{
    private const string ConsoleLogName = "Console";

    public static LogRegistration Create(IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<ConsoleLogOptions>>().Value;

        if (!options.Enabled)
            return LogRegistration.NullRegistration;

        var settings = new ConsoleLogSettings { ColorsEnabled = true };
        options.ApplyCustomizations(settings);

        ILog log = options.Synchronous ? new SynchronousConsoleLog(settings) : new ConsoleLog(settings);

        if (options.MinLevelProvider is not null)
            log = log.WithMinimumLevel(options.MinLevelProvider);

        // todo: Customize log

        return new LogRegistration(log, ConsoleLogName, new ActionDisposable(ConsoleLog.Flush));
    }
}