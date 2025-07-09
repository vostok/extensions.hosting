using JetBrains.Annotations;

using Vostok.Extensions.Hosting.Components.Logging.Internal;
using Vostok.Logging.Abstractions;

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
}