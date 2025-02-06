using JetBrains.Annotations;

using Vostok.Metrics;

namespace Vostok.Extensions.Hosting.Components.Metrics;

[PublicAPI]
public sealed class MetricsConfigurationOptions
{
    private readonly List<Action<MetricContextConfig>> _customizations = [];
    private readonly List<Func<IAnnotationEventSender, IAnnotationEventSender>> _annotationSenderCustomizations = [];

    public void CustomizeSettings(Action<MetricContextConfig> customization)
        => _customizations.Add(customization);

    public void CustomizeAnnotationEventSender(Func<IAnnotationEventSender, IAnnotationEventSender> customization)
        => _annotationSenderCustomizations.Add(customization);

    internal void ApplyCustomizations(MetricContextConfig settings) =>
        _customizations.ForEach(customization => customization(settings));

    internal IAnnotationEventSender ApplyAnnotationSenderCustomizations(IAnnotationEventSender annotationSender)
    {
        foreach (var customization in _annotationSenderCustomizations)
            annotationSender = customization(annotationSender);

        return annotationSender;
    }
}