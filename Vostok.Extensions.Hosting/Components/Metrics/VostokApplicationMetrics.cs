using Vostok.Commons.Environment;
using Vostok.Hosting.Abstractions;
using Vostok.Metrics;

namespace Vostok.Extensions.Hosting.Components.Metrics;

internal sealed class VostokApplicationMetrics : IVostokApplicationMetrics, IDisposable
{
    private bool _disposed;

    public IMetricContext Root { get; }
    public IMetricContext Project { get; }
    public IMetricContext Subproject { get; }
    public IMetricContext Environment { get; }
    public IMetricContext Application { get; }
    public IMetricContext Instance { get; }

    public VostokApplicationMetrics(IMetricContext root, IVostokApplicationIdentity identity)
    {
        Root = root;
        Project = Root.WithTag(WellKnownApplicationIdentityProperties.Project, identity.Project);

        Subproject = identity.Subproject == null
            ? Project
            : Project.WithTag(WellKnownApplicationIdentityProperties.Subproject, identity.Subproject);

        Environment = Subproject.WithTag(WellKnownApplicationIdentityProperties.Environment, identity.Environment);
        Application = Environment.WithTag(WellKnownApplicationIdentityProperties.Application, identity.Application);

        var instance = identity.Instance;
        if (string.Equals(instance, EnvironmentInfo.Host, StringComparison.InvariantCultureIgnoreCase))
            instance = instance.ToLowerInvariant();

        Instance = Application.WithTag(WellKnownApplicationIdentityProperties.Instance, instance);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        if (Root is IDisposable disposable)
            disposable.Dispose();

        _disposed = true;
    }
}