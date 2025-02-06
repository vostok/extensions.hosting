using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Hercules.Client.Abstractions;
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

        // Todo: Enable/Disable
        if (!options.Enabled)
            log.LogDisabled("HerculesMetricSender", "explicit disable");

        var setting = new HerculesMetricSenderSettings(herculesSink)
        {
            Enabled = () => options.Enabled
        };

        options.ApplyCustomizations(setting);

        return new HerculesMetricSender(setting);
    }
}