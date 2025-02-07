using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Hercules.Client.Abstractions;
using Vostok.Hercules.Client.Abstractions.Models;
using Vostok.Logging.Abstractions;
using Vostok.ServiceDiscovery.Telemetry.Hercules;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

public static class ServiceBeaconConfigurationOptionsExtensions
{
    public static void AddHerculesEventsSender(
        this ServiceBeaconConfigurationOptions options,
        Action<HerculesServiceDiscoveryEventsSenderOptions> configure)
    {
        options.SetEventsSender(provider => Create(provider, configure));
    }

    private static HerculesServiceDiscoveryEventsSender? Create(
        IServiceProvider services,
        Action<HerculesServiceDiscoveryEventsSenderOptions> configure)
    {
        var log = services.GetRequiredService<ILog>();
        var options = services.GetRequiredService<IOptions<HerculesServiceDiscoveryEventsSenderOptions>>().Value;

        configure(options);

        if (!options.Enabled)
        {
            log.LogDisabled(nameof(HerculesServiceDiscoveryEventsSender), "explicit disable");
            return null;
        }

        var sink = services.GetService<IHerculesSink>();

        if (sink == null)
        {
            log.LogDisabled(nameof(HerculesServiceDiscoveryEventsSender), "disabled HerculesSink");
            return null;
        }

        if (options.Stream is null)
        {
            log.LogDisabled(nameof(HerculesServiceDiscoveryEventsSender), "not configured stream");
            return null;
        }

        sink.ConfigureStream(options.Stream, new StreamSettings { ApiKeyProvider = options.ApiKeyProvider });

        var settings = new HerculesServiceDiscoveryEventsSenderSettings(sink, options.Stream);
        return new HerculesServiceDiscoveryEventsSender(settings);
    }
}