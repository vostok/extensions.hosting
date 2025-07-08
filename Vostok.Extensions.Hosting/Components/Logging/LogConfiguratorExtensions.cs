using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Vostok.Extensions.Hosting.Components.Logging.Internal;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Configuration;

namespace Vostok.Extensions.Hosting.Components.Logging;

[PublicAPI]
public static class LogConfiguratorExtensions
{
    public static ILogConfigurator SetMinimumLevelProvider(
        this ILogConfigurator configurator,
        Func<LogLevel> minLevelProvider)
    {
        configurator.Services.ConfigureMiscellaneousOptions(
            options => options.MinLevelProvider = minLevelProvider,
            configurator.Name);

        return configurator;
    }

    public static ILogConfigurator CustomizeLog(
        this ILogConfigurator configurator,
        Func<ILog, ILog> customize) =>
        configurator.CustomizeLog((log, _) => customize(log));

    public static ILogConfigurator CustomizeLog(
        this ILogConfigurator configurator,
        Func<ILog, IServiceProvider, ILog> customize)
    {
        var customization = new LogCustomization(customize);
        configurator.Services.ConfigureMiscellaneousOptions(
            options => options.Customizations.Add(customization),
            configurator.Name);

        return configurator;
    }

    public static ILogConfigurator AddRule(this ILogConfigurator configurator, LogConfigurationRule rule)
    {
        rule = rule.WithLog(configurator.Name);
        configurator.Services.Configure<VostokLogRulesOptions>(options => options.Rules.Add(rule));

        return configurator;
    }
}