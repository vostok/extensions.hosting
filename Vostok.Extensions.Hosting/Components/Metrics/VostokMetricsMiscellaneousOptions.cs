using Vostok.Metrics;

namespace Vostok.Extensions.Hosting.Components.Metrics;

internal sealed class VostokMetricsMiscellaneousOptions
{
    public List<Func<IAnnotationEventSender, IAnnotationEventSender>> AnnotationsSenderCustomizations { get; } = [];
}