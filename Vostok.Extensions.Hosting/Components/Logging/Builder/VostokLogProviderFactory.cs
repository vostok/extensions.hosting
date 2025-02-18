using Microsoft.Extensions.DependencyInjection;

using Vostok.Logging.Abstractions;
using Vostok.Logging.Configuration;
using Vostok.Logging.Microsoft;

namespace Vostok.Extensions.Hosting.Components.Logging.Builder;

internal static class VostokLogProviderFactory
{
    public static VostokLoggerProvider Create(IServiceProvider provider)
    {
        var logs = provider.GetServices<LogRegistration>().Where(r => r.IsEnabled).ToArray();

        var configurableLogBuilder = new ConfigurableLogBuilder();

        foreach (var log in logs)
            configurableLogBuilder.AddLog(log.Name, log.Log);

        // todo: Set rules after configuration implementation.
        // configurableLogBuilder.SetRules();

        ILog compositeLog = configurableLogBuilder.Build();

        // var compositeLog = new CompositeLog(logs);

        return new VostokLoggerProvider(compositeLog);
    }
}