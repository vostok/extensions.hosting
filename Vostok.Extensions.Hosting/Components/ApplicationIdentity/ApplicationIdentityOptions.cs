using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace Vostok.Extensions.Hosting.Components.ApplicationIdentity;

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