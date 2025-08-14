using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ApplicationIdentity;

/// <summary>
/// Options for configuring <see cref="IVostokApplicationIdentity"/>.
/// </summary>
[PublicAPI]
public sealed class ApplicationIdentityOptions
{
    [DisallowNull]
    public string? Project { get; set; }

    public string? Subproject { get; set; }

    [DisallowNull]
    public string? Environment { get; set; }

    [DisallowNull]
    public string? Application { get; set; }

    [DisallowNull]
    public string? Instance { get; set; }
}