using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ApplicationReplicationInfo;

public sealed class ApplicationReplicationInfoConfigurationOptions
{
    private Func<IVostokApplicationReplicationInfo> _replicationInfoProvider =
        () => new VostokApplicationReplicationInfo(0, 1);

    public void SetReplicationInfo(int instanceIndex, int instancesCount)
    {
        var replicationInfo = new VostokApplicationReplicationInfo(instanceIndex, instancesCount);

        _replicationInfoProvider = () => replicationInfo;
    }

    public void SetReplicationInfoProvider(Func<IVostokApplicationReplicationInfo> provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        _replicationInfoProvider = provider;
    }

    internal Func<IVostokApplicationReplicationInfo> GetProvider() => _replicationInfoProvider;
}