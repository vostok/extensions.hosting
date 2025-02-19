using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Components.Logging.Builder;
using Vostok.Logging.Abstractions;
using Vostok.Logging.File;
using Vostok.Logging.File.Configuration;

namespace Vostok.Extensions.Hosting.Components.Logging.File;

internal static class FileLogFactory
{
    private const string FileLogName = "File";

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

        return new LogRegistration(log, FileLogName);
    }
}