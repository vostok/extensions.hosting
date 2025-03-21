using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Metrics.Hercules;

namespace Vostok.Extensions.Hosting.Components.Metrics.Hercules;

internal static class HerculesMetricSenderFactory
{
    public static HerculesMetricSender Create(IServiceProvider provider)
    {
        var setting = provider.GetRequiredService<IOptions<HerculesMetricSenderSettings>>().Value;
        return new HerculesMetricSender(setting);
    }
}