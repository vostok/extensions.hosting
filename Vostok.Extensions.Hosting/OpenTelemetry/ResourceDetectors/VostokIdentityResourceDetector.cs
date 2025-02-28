using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.DependencyInjection;

using OpenTelemetry.Resources;

using Vostok.Commons.Environment;
using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.OpenTelemetry.ResourceDetectors;

[SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeNotEvident")]
internal sealed class VostokIdentityResourceDetector(IServiceProvider provider) : IResourceDetector
{
    private readonly IVostokApplicationIdentity? _identity = provider.GetService<IVostokApplicationIdentity>();

    public Resource Detect()
    {
        if (_identity is null)
            return Resource.Empty;

        var attributes = new List<KeyValuePair<string, object>>();

        attributes.Add(new(WellKnownApplicationIdentityProperties.Project, _identity.Project));
        if (_identity.Subproject is not null)
            attributes.Add(new(WellKnownApplicationIdentityProperties.Subproject, _identity.Subproject));
        attributes.Add(new(WellKnownApplicationIdentityProperties.Environment, _identity.Environment));
        attributes.Add(new(WellKnownApplicationIdentityProperties.Application, _identity.Application));

        var instance = _identity.Instance;
        if (string.Equals(instance, EnvironmentInfo.Host, StringComparison.InvariantCultureIgnoreCase))
            instance = instance.ToLowerInvariant();
        attributes.Add(new(WellKnownApplicationIdentityProperties.Instance, instance));

        return new Resource(attributes);
    }
}