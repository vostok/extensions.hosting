using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Commons.Helpers.Disposable;
using Vostok.Extensions.Hosting.Components.Logging.Builder;
using Vostok.Logging.Abstractions;
using Vostok.Logging.File;
using Vostok.Logging.File.Configuration;

namespace Vostok.Extensions.Hosting.Components.Logging.File;

internal static class FileLogFactory
{
    public const string LogName = "File";

    public static ILog Create(IServiceProvider provider, object? _)
    {
        var settingsMonitor = provider.GetRequiredService<IOptionsMonitor<FileLogSettings>>();
        var customizations = provider.GetKeyedServices<LogCustomization>(LogName);

        var log = new FileLog(() => settingsMonitor.CurrentValue);

        ILog finalLog = log;
        foreach (var customization in customizations)
            finalLog = customization(finalLog, provider);

        if (ReferenceEquals(log, finalLog))
            return log;

        return new DisposableLogWrapper(finalLog, new ActionDisposable(log.Dispose));
    }
}