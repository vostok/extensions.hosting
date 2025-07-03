using Microsoft.Extensions.DependencyInjection;

using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Helpers;

internal static class VostokBuilderLoggingHelper
{
    public static void LogDisabled(this ILog log, string name, string reason) =>
        log.ForContext<VostokBuilder>()
           .Info("{ComponentName} feature has been disabled due to {ComponentDisabledReason}.", name, reason);

    public static ILog GetInitializationLog(this IServiceProvider provider) =>
        provider.GetRequiredKeyedService<ILog>(InitializationLog.Key);
}