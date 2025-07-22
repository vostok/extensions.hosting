using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

using Vostok.Configuration;
using Vostok.Configuration.Logging;
using Vostok.Configuration.Sources.Manual;
using Vostok.Extensions.Hosting.Helpers;

using ConfigurationProvider = Vostok.Configuration.ConfigurationProvider;
using IConfigurationProvider = Vostok.Configuration.Abstractions.IConfigurationProvider;
using IConfigurationSource = Vostok.Configuration.Abstractions.IConfigurationSource;

namespace Vostok.Extensions.Hosting.Components.Configuration;

internal static class ConfigurationFactory
{
    public static IConfigurationSource CreateSource(IServiceProvider provider)
    {
        var configuration = provider.GetRequiredService<IConfiguration>();

        var source = new ManualFeedSource();
        PushCurrentConfiguration();

        ChangeToken.OnChange(configuration.GetReloadToken, PushCurrentConfiguration);

        return source;

        void PushCurrentConfiguration() =>
            source.Push(ConfigurationConverter.ConvertToSettingsNode(configuration));
    }

    public static IConfigurationProvider CreateProvider(IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<VostokConfigurationMiscellaneousOptions>>().Value;
        var configurationSource = provider.GetRequiredService<IConfigurationSource>();

        var log = provider.GetInitializationLog();
        var settings = new ConfigurationProviderSettings()
                       .WithErrorLogging(log)
                       .WithUpdatesLogging(log);

        var configurationProvider = new ConfigurationProvider(settings);
        foreach (var customize in options.Customizations)
            customize(configurationProvider, configurationSource);

        return configurationProvider;
    }
}