using Microsoft.Extensions.Configuration;

using Vostok.ClusterConfig.Client.Abstractions;
using Vostok.Configuration.Sources.ClusterConfig;

namespace Vostok.Extensions.Hosting.Configuration;

internal sealed class ClusterConfigDeferredConfigurationSource(
    string prefix,
    Action<ClusterConfigSourceSettings>? configure) : IConfigurationSource
{
    private static readonly List<Registration> Registrations = [];

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        var provider = new ClusterConfigDeferredConfigurationProvider();
        Registrations.Add(new Registration(provider, prefix, configure));

        return provider;
    }

    public static void ActivateAllProviders(IClusterConfigClient newClient)
    {
        foreach (var (provider, prefix, configure) in Registrations)
        {
            var settings = new ClusterConfigSourceSettingsWithParsers(newClient, prefix);
            configure?.Invoke(settings);

            provider.Activate(new ClusterConfigSource(settings));
        }
    }

    private sealed record Registration(
        ClusterConfigDeferredConfigurationProvider Provider,
        string Prefix,
        Action<ClusterConfigSourceSettings>? ConfigureSettings);
}