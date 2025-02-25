using Microsoft.Extensions.DependencyInjection;

using Vostok.Commons.Environment;
using Vostok.Extensions.Hosting.OpenTelemetry;
using Vostok.Hosting.Abstractions;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Tracing;
using Vostok.Tracing.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Logging;

internal static class LogCustomizationExtensions
{
    public static ILog WithVostokSpecificProperties(this ILog log, IServiceProvider provider)
    {
        var applicationIdentity = provider.GetRequiredService<IVostokApplicationIdentity>();
        var tracer = provider.GetRequiredService<ITracer>();

        return log.WithApplicationIdentityProperties(applicationIdentity)
                  .WithTracingProperties(tracer)
                  .WithProperty(VostokSemanticConventions.AttributeHostName, EnvironmentInfo.Host);
    }

    private static ILog WithApplicationIdentityProperties(this ILog log, IVostokApplicationIdentity applicationIdentity)
    {
        log = log.WithProperty(WellKnownApplicationIdentityProperties.Project, applicationIdentity.Project);
        if (applicationIdentity.Subproject != null)
            log = log.WithProperty(WellKnownApplicationIdentityProperties.Subproject, applicationIdentity.Subproject);
        log = log.WithProperty(WellKnownApplicationIdentityProperties.Environment, applicationIdentity.Environment);
        log = log.WithProperty(WellKnownApplicationIdentityProperties.Application, applicationIdentity.Application);
        log = log.WithProperty(WellKnownApplicationIdentityProperties.Instance, applicationIdentity.Instance);

        return log;
    }
}