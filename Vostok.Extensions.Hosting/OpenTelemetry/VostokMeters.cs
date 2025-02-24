using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

using JetBrains.Annotations;

using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.OpenTelemetry;

[PublicAPI]
public sealed class VostokMeters
{
    public KeyValuePair<string, object?>[] ProjectTags { get; }
    public KeyValuePair<string, object?>[] SubprojectTags { get; }
    public KeyValuePair<string, object?>[] EnvironmentTags { get; }
    public KeyValuePair<string, object?>[] ApplicationTags { get; }
    public KeyValuePair<string, object?>[] InstanceTags { get; }

    public Meter Project { get; }
    public Meter Subproject { get; }
    public Meter Environment { get; }
    public Meter Application { get; }
    public Meter Instance { get; }

    [SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeNotEvident")]
    public VostokMeters(IVostokApplicationIdentity identity, IMeterFactory meterFactory)
    {
        ProjectTags = [new(WellKnownApplicationIdentityProperties.Project, identity.Project)];
        SubprojectTags = identity.Subproject is null
            ? ProjectTags
            : [..ProjectTags, new(WellKnownApplicationIdentityProperties.Project, identity.Subproject)];
        EnvironmentTags = [..SubprojectTags, new(WellKnownApplicationIdentityProperties.Environment, identity.Environment)];
        ApplicationTags = [..EnvironmentTags, new(WellKnownApplicationIdentityProperties.Application, identity.Application)];
        InstanceTags = [..ApplicationTags, new(WellKnownApplicationIdentityProperties.Instance, identity.Instance)];

        var version = typeof(VostokMeters).Assembly.GetName().Version?.ToString();

        Project = meterFactory.Create("Vostok.Metrics.Project", version, ProjectTags);
        Subproject = meterFactory.Create("Vostok.Metrics.Subproject", version, SubprojectTags);
        Environment = meterFactory.Create("Vostok.Metrics.Environment", version, EnvironmentTags);
        Application = meterFactory.Create("Vostok.Metrics.Application", version, ApplicationTags);
        Instance = meterFactory.Create("Vostok.Metrics.Instance", version, InstanceTags);
    }
}