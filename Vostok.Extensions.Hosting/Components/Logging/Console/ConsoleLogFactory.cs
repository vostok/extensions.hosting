using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Commons.Helpers.Disposable;
using Vostok.Extensions.Hosting.Components.Logging.Builder;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Console;

namespace Vostok.Extensions.Hosting.Components.Logging.Console;

internal static class ConsoleLogFactory
{
    public const string LogName = "Console";

    public static ILog Create(IServiceProvider provider, object? _)
    {
        var settings = provider.GetRequiredService<IOptions<ConsoleLogSettings>>().Value;
        var customizations = provider.GetKeyedServices<LogCustomization>(LogName);

        ILog log = new ConsoleLog(settings);

        foreach (var customization in customizations)
            log = customization(log, provider);

        return new DisposableLogWrapper(log, new ActionDisposable(ConsoleLog.Flush));
    }
}