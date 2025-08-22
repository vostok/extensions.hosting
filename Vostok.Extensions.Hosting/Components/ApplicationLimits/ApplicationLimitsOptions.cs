using JetBrains.Annotations;

using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ApplicationLimits;

/// <summary>
/// Options for configuring <see cref="IVostokApplicationLimits"/>.
/// </summary>
[PublicAPI]
public sealed class ApplicationLimitsOptions
{
    public float? CpuUnits { get; set; }
    public long? MemoryBytes { get; set; }
}