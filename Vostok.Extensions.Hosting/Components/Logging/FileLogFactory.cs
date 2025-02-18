using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Commons.Helpers.Disposable;
using Vostok.Extensions.Hosting.Components.Logging.Builder;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Console;
using Vostok.Logging.File;
using Vostok.Logging.File.Configuration;

namespace Vostok.Extensions.Hosting.Components.Logging;

internal static class FileLogFactory
{
    public static LogRegistration Create(IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<FileLogOptions>>().Value;

        if (!options.Enabled)
            return LogRegistration.NullRegistration;

        var settings = new FileLogSettings();
        options.ApplyCustomizations(settings);

        ILog log = new FileLog(() => settings);

        if (options.MinLevelProvider is not null)
            log = log.WithMinimumLevel(options.MinLevelProvider);

        // todo: Customize log
        // todo: Log directory
        
        return new LogRegistration(log, new ActionDisposable(ConsoleLog.Flush));
    }
}