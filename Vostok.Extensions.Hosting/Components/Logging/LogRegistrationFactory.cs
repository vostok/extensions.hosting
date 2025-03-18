using Microsoft.Extensions.DependencyInjection;

using Vostok.Commons.Helpers.Disposable;
using Vostok.Extensions.Hosting.Components.Logging.Builder;
using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Logging;

internal static class LogRegistrationFactory
{
    private static int unnamedLogsCounter;

    public static string GetNextName() => $"Log-{Interlocked.Increment(ref unnamedLogsCounter)}";

    public static ILog Register(
        IServiceProvider provider,
        Func<IServiceProvider, ILog> implementationFactory,
        string logName,
        Action? customDispose)
    {
        var log = implementationFactory(provider);
        var customizations = provider.GetKeyedServices<LogCustomization>(logName);

        foreach (var customization in customizations)
            log = customization(log, provider);

        if (log is not IDisposable && customDispose is not null)
            return new DisposableLogWrapper(log, new ActionDisposable(customDispose));
        return log;
    }
}