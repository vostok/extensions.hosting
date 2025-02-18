using Microsoft.Extensions.DependencyInjection;

using Vostok.Logging.Abstractions;
using Vostok.Logging.Microsoft;

namespace Vostok.Extensions.Hosting.Components.Logging.Builder;

internal static class VostokLogProviderFactory
{
    public static VostokLoggerProvider Create(IServiceProvider provider)
    {
        var logs = provider.GetServices<LogRegistration>()
                           .Where(r => r.Log is not null)
                           .Select(r => r.Log!)
                           .ToArray();

        var compositeLog = new CompositeLog(logs);

        return new VostokLoggerProvider(compositeLog);
    }
}