using Vostok.Metrics;

namespace Vostok.Extensions.Hosting.Components.Metrics;

internal sealed class VostokMetricsMiscellaneousOptions
{
    public ICollection<Func<IAnnotationEventSender, IServiceProvider, IAnnotationEventSender>> AnnotationsSenderCustomizations { get; } = [];
}