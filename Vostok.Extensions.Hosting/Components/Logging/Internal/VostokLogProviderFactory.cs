using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Commons.Helpers.Observable;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Configuration;
using Vostok.Logging.Microsoft;

namespace Vostok.Extensions.Hosting.Components.Logging.Internal;

internal static class VostokLogProviderFactory
{
    public const string InternalLogKey = "_InternalLog";

    public static VostokLoggerProvider Create(IServiceProvider provider)
    {
        var vostokLog = provider.GetRequiredKeyedService<ILog>(InternalLogKey);
        var settings = provider.GetRequiredService<IOptions<VostokLoggerProviderSettings>>().Value;

        return new VostokLoggerProvider(vostokLog, settings);
    }

    public static ILog CreateInternalVostokLog(IServiceProvider provider)
    {
        var vostokLog = GetVostokLog(provider);
        return vostokLog is SilentLog
            ? vostokLog
            : new InternalDisposableVostokLog(vostokLog);
    }

    private static ILog GetVostokLog(IServiceProvider provider)
    {
        var logRegistrations = provider.GetKeyedServices<LogRegistration>(LogRegistration.Key.All).ToArray();
        if (logRegistrations.Length is 0)
            return new SilentLog();

        var builder = new ConfigurableLogBuilder();

        foreach (var registration in logRegistrations)
            builder.AddLog(registration.LogName, registration.Create(provider));

        var rulesMonitor = provider.GetRequiredService<IOptionsMonitor<VostokLogRulesOptions>>();
        var rulesObservable = GetRulesObservable(rulesMonitor);
        builder.SetRules(rulesObservable);

        var configurableLog = builder.Build();
        configurableLog.WaitForRulesInitialization(TimeSpan.FromSeconds(1));

        ILog finalLog = configurableLog;

        var options = provider.GetRequiredService<IOptionsSnapshot<VostokLogMiscellaneousOptions>>().Value;
        foreach (var customization in options.Customizations)
            finalLog = customization(finalLog, provider);

        if (options.MinLevelProvider is not null)
            finalLog = finalLog.WithMinimumLevel(options.MinLevelProvider);

        return finalLog;
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