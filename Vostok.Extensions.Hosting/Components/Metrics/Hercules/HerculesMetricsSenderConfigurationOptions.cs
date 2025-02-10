using JetBrains.Annotations;

using Vostok.Metrics.Hercules;

namespace Vostok.Extensions.Hosting.Components.Metrics.Hercules;

[PublicAPI]
public sealed class HerculesMetricsSenderConfigurationOptions
{
    internal readonly List<(string? stream, Func<string?> apiKeyProvider)> ApiKeyProviders = [];

    private readonly List<Action<HerculesMetricSenderSettings>> _customizations = [];

    public bool Enabled { get; set; } = true;

    public void CustomizeSettings(Action<HerculesMetricSenderSettings> settingsCustomization) =>
        _customizations.Add(settingsCustomization);

    public void SetApiKeyProvider(Func<string?> apiKeyProvider, string? stream = null) =>
        ApiKeyProviders.Add((stream, apiKeyProvider));

    internal void ApplyCustomizations(HerculesMetricSenderSettings settings) =>
        _customizations.ForEach(customization => customization(settings));
}