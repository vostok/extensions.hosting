using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Commons.Helpers.Observable;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Configuration;
using Vostok.Logging.Microsoft;

namespace Vostok.Extensions.Hosting.Components.Logging.Internal;

internal static class VostokLogProviderFactory
{
    public static VostokLoggerProvider Create(IServiceProvider provider)
    {
        var logs = provider.GetKeyedServices<ILog>(LogRegistration.All).ToArray();
        var names = provider.GetKeyedServices<LogRegistration>(LogRegistration.All).Select(r => r.Name).ToArray();
        if (logs.Length != names.Length)
            throw new InvalidOperationException("Logs and names collections lengths mismatch.");

        var builder = new ConfigurableLogBuilder();

        foreach (var (log, name) in logs.Zip(names)) // Logs and names registered together so they are in same order
            builder.AddLog(name, log);

        var rulesMonitor = provider.GetRequiredService<IOptionsMonitor<VostokLogRulesOptions>>();
        var rulesObservable = GetRulesObservable(rulesMonitor);
        builder.SetRules(rulesObservable);

        var configurableLog = builder.Build();
        configurableLog.WaitForRulesInitialization(TimeSpan.FromSeconds(1));

        ILog finalLog = configurableLog;
        var customizations = provider.GetServices<LogCustomization>();
        foreach (var customization in customizations)
            finalLog = customization(finalLog, provider);

        var settings = provider.GetRequiredService<IOptions<VostokLoggerProviderSettings>>().Value;
        return new VostokLoggerProvider(finalLog, settings);
    }

    private static CachingObservable<LogConfigurationRule[]> GetRulesObservable(
        IOptionsMonitor<VostokLogRulesOptions> optionsMonitor)
    {
        var initialOptions = optionsMonitor.CurrentValue;
        var observable = new CachingObservable<LogConfigurationRule[]>(initialOptions.Rules.ToArray());

        optionsMonitor.OnChange(newOptions => observable.Next(newOptions.Rules.ToArray()));

        return observable;
    }
}