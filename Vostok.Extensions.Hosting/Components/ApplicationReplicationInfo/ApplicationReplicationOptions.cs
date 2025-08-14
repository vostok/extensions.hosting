using JetBrains.Annotations;

using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ApplicationReplicationInfo;

/// <summary>
/// Options for configuring <see cref="IVostokApplicationReplicationInfo"/>.
/// </summary>
[PublicAPI]
public sealed class ApplicationReplicationOptions
{
    public int InstanceIndex { get; set; } = 0;
    public int InstancesCount { get; set; } = 1;
}