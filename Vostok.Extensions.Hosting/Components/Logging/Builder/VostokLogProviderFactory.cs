using Microsoft.Extensions.DependencyInjection;

using Vostok.Logging.Abstractions;
using Vostok.Logging.Microsoft;

namespace Vostok.Extensions.Hosting.Components.Logging.Builder;

internal static class VostokLogProviderFactory
{
    public static VostokLoggerProvider Create(IServiceProvider provider)
    {
        var logs = provider.GetKeyedServices<ILog>(LogRegistration.All).ToArray();

        var customizations = provider.GetServices<LogCustomization>();

        ILog compositeLog = new CompositeLog(logs);

        foreach (var customization in customizations)
            compositeLog = customization(compositeLog, provider);

        return new VostokLoggerProvider(compositeLog);
    }
}