using Microsoft.Extensions.Diagnostics.Enrichment;

using Vostok.Commons.Environment;
using Vostok.Extensions.Hosting.OpenTelemetry;
using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Logging;

internal sealed class ApplicationInfoEnricher(IVostokApplicationIdentity identity) : IStaticLogEnricher
{
    public void Enrich(IEnrichmentTagCollector collector)
    {
        collector.Add(WellKnownApplicationIdentityProperties.Project, identity.Project);
        if (identity.Subproject != null)
            collector.Add(WellKnownApplicationIdentityProperties.Subproject, identity.Subproject);
        collector.Add(WellKnownApplicationIdentityProperties.Environment, identity.Environment);
        collector.Add(WellKnownApplicationIdentityProperties.Application, identity.Application);
        collector.Add(WellKnownApplicationIdentityProperties.Instance, identity.Instance);

        collector.Add(SemanticConventions.AttributeHostName, EnvironmentInfo.Host);
    }
}