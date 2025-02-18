using Microsoft.Extensions.DependencyInjection;

using OpenTelemetry.Resources;

using Vostok.Commons.Environment;
using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.OpenTelemetry.ResourceDetectors;

internal sealed class VostokIdentityResourceDetector(
    IServiceProvider provider,
    VostokIdentityResourceDetector.AttributesOptions? options = null) : IResourceDetector
{
    private readonly IVostokApplicationIdentity? _identity = provider.GetService<IVostokApplicationIdentity>();
    private readonly AttributesOptions _options = options ?? new AttributesOptions();

    public Resource Detect()
    {
        if (_identity is null)
            return Resource.Empty;

        var attributes = new List<KeyValuePair<string, object>>();

        if (_options.AddProject)
            attributes.Add(new(WellKnownApplicationIdentityProperties.Project, _identity.Project));
        if (_options.AddSubproject && _identity.Subproject is not null)
            attributes.Add(new(WellKnownApplicationIdentityProperties.Subproject, _identity.Subproject));
        if (_options.AddEnvironment)
            attributes.Add(new(WellKnownApplicationIdentityProperties.Environment, _identity.Environment));
        if (_options.AddApplication)
            attributes.Add(new(WellKnownApplicationIdentityProperties.Application, _identity.Application));

        if (_options.AddInstance)
        {
            var instance = _identity.Instance;
            if (string.Equals(instance, EnvironmentInfo.Host, StringComparison.InvariantCultureIgnoreCase))
                instance = instance.ToLowerInvariant();
            attributes.Add(new(WellKnownApplicationIdentityProperties.Instance, instance));
        }

        return new Resource(attributes);
    }

    public class AttributesOptions
    {
        public bool AddProject { get; init; } = true;
        public bool AddSubproject { get; init; } = true;
        public bool AddEnvironment { get; init; } = true;
        public bool AddApplication { get; init; } = true;
        public bool AddInstance { get; init; } = true;
    }
}