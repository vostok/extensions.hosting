using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Hercules.Client.Abstractions;
using Vostok.Hercules.Client.Abstractions.Models;
using Vostok.Logging.Abstractions;
using Vostok.Metrics.Hercules;

namespace Vostok.Extensions.Hosting.Components.Metrics.Hercules;

internal static class HerculesMetricSenderFactory
{
    public static HerculesMetricSender Create(IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<HerculesMetricsSenderConfigurationOptions>>().Value;
        var herculesSink = provider.GetRequiredService<IHerculesSink>();
        var log = provider.GetRequiredService<ILog>();

        if (!options.Enabled)
            log.LogDisabled("HerculesMetricSender", "explicit disable");

        var setting = new HerculesMetricSenderSettings(herculesSink)
        {
            Enabled = () => options.Enabled
        };

        options.ApplyCustomizations(setting);

        ConfigureStreams(options, setting, herculesSink);

        return new HerculesMetricSender(setting);
    }

    private static void ConfigureStreams(
        HerculesMetricsSenderConfigurationOptions options,
        HerculesMetricSenderSettings settings,
        IHerculesSink herculesSink)
    {
        var allStreams = new[]
        {
            settings.FallbackStream,
            settings.FinalStream,
            settings.CountersStream,
            settings.TimersStream,
            settings.HistogramsStream,
            settings.AnnotationsStream
        };

        foreach (var (stream, apiKeyProvider) in options.ApiKeyProviders)
        {
            if (stream != null)
                herculesSink.ConfigureStream(stream, new StreamSettings { ApiKeyProvider = apiKeyProvider });
            else
            {
                foreach (var s in allStreams)
                    herculesSink.ConfigureStream(s!, new StreamSettings { ApiKeyProvider = apiKeyProvider });
            }
        }
    }
}