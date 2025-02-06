using JetBrains.Annotations;

using Vostok.Metrics.Hercules;

namespace Vostok.Extensions.Hosting.Components.Metrics.Hercules;

[PublicAPI]
public sealed class HerculesMetricsSenderConfigurationOptions
{
    private readonly List<Action<HerculesMetricSenderSettings>> _customizations = [];

    public bool Enabled { get; set; } = true;
    public Func<string>? ApiKeyProvider { get; set; }

    public void CustomizeSettings(Action<HerculesMetricSenderSettings> settingsCustomization) =>
        _customizations.Add(settingsCustomization);

    internal void ApplyCustomizations(HerculesMetricSenderSettings settings) =>
        _customizations.ForEach(customization => customization(settings));
}