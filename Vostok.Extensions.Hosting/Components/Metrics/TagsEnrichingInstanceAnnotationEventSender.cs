using Vostok.Hosting.Abstractions;
using Vostok.Metrics;
using Vostok.Metrics.Models;

namespace Vostok.Extensions.Hosting.Components.Metrics;

internal sealed class TagsEnrichingInstanceAnnotationEventSender(
    IAnnotationEventSender baseSender,
    MetricTags tagsToAppend) : IAnnotationEventSender
{
    public void Send(AnnotationEvent @event)
    {
        if (@event.Tags.Any(metricTag => metricTag.Key == WellKnownApplicationIdentityProperties.Instance))
        {
            var enrichedTags = @event.Tags.Append(tagsToAppend);
            @event = new AnnotationEvent(@event.Timestamp, enrichedTags, @event.Description);
        }

        baseSender.Send(@event);
    }
}