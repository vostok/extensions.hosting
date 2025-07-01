using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Logging.File;
using Vostok.Logging.File.Configuration;

namespace Vostok.Extensions.Hosting.Components.Logging.File;

internal static class FileLogFactory
{
    public const string LogName = "File";

    public static FileLog Create(IServiceProvider provider)
    {
        var settingsMonitor = provider.GetRequiredService<IOptionsMonitor<FileLogSettings>>();
        return new FileLog(() => settingsMonitor.CurrentValue);
    }
}