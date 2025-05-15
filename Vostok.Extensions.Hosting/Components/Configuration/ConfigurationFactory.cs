using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

using Vostok.Configuration.Sources.Manual;

using ConfigurationProvider = Vostok.Configuration.ConfigurationProvider;
using IConfigurationProvider = Vostok.Configuration.Abstractions.IConfigurationProvider;
using IConfigurationSource = Vostok.Configuration.Abstractions.IConfigurationSource;

namespace Vostok.Extensions.Hosting.Components.Configuration;

internal delegate void ConfigurationProviderSetup(IConfigurationProvider provider, IConfigurationSource source);

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
        var setups = provider.GetServices<ConfigurationProviderSetup>();
        var configurationSource = provider.GetRequiredService<IConfigurationSource>();

        var configurationProvider = new ConfigurationProvider();
        foreach (var setup in setups)
            setup(configurationProvider, configurationSource);

        // todo: Add error logging

        return configurationProvider;
    }
}