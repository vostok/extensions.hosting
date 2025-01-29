using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting;

public interface IVostokContext
{
    IVostokApplicationIdentity ApplicationIdentity { get; }
    IVostokApplicationReplicationInfo ApplicationReplicationInfo { get; }
}