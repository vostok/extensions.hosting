using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting;

internal sealed class VostokContext(
    IVostokApplicationIdentity applicationIdentity,
    Func<IVostokApplicationReplicationInfo> applicationReplicationInfoProvider) : IVostokContext
{
    public IVostokApplicationIdentity ApplicationIdentity { get; } = applicationIdentity;
    public IVostokApplicationReplicationInfo ApplicationReplicationInfo => applicationReplicationInfoProvider();
}