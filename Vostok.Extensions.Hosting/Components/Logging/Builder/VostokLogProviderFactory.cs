using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Logging.Abstractions;
using Vostok.Logging.Configuration;
using Vostok.Logging.Microsoft;

namespace Vostok.Extensions.Hosting.Components.Logging.Builder;

internal static class VostokLogProviderFactory
{
    public static VostokLoggerProvider Create(IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<LogConfigurationOptions>>().Value;
        var logs = provider.GetServices<LogRegistration>().Where(r => r.IsEnabled).ToArray();
        var rules = provider.GetServices<LogConfigurationRule>().ToArray();

        var configurableLogBuilder = new ConfigurableLogBuilder();

        foreach (var log in logs)
            configurableLogBuilder.AddLog(log.Name, log.Log);

        // todo: Set rules sources after configuration implementation.
        configurableLogBuilder.SetRules(rules);

        ILog compositeLog = configurableLogBuilder.Build();
        compositeLog = options.ApplyLogCustomizations(compositeLog);

        return new VostokLoggerProvider(compositeLog);
    }
}