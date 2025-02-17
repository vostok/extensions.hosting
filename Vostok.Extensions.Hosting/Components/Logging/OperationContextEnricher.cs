using Microsoft.Extensions.Diagnostics.Enrichment;

using Vostok.Context;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Abstractions.Values;

namespace Vostok.Extensions.Hosting.Components.Logging;

internal sealed class OperationContextEnricher : ILogEnricher
{
    public void Enrich(IEnrichmentTagCollector collector)
    {
        var operationContext = FlowingContext.Globals.Get<OperationContextValue>();
        if (operationContext is null)
            return;

        collector.Add(WellKnownProperties.OperationContext, operationContext);

        if (operationContext.Properties is null || operationContext.Properties.Count == 0)
            return;

        foreach (var (key, value) in operationContext.Properties)
            collector.Add(key, value);
    }
}