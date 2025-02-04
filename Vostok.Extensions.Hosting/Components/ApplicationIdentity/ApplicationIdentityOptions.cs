using JetBrains.Annotations;

using static System.Environment;

namespace Vostok.Extensions.Hosting.Components.ApplicationIdentity;

[PublicAPI]
public sealed class ApplicationIdentityOptions
{
    public string? Project { get; set; } = GetEnvironmentVariable(VostokEnvironmentVariables.IdentityProject);
    public string? Subproject { get; set; } = GetEnvironmentVariable(VostokEnvironmentVariables.IdentitySubproject);
    public string? Environment { get; set; } = GetEnvironmentVariable(VostokEnvironmentVariables.IdentityEnvironment);
    public string? Application { get; set; } = GetEnvironmentVariable(VostokEnvironmentVariables.IdentityApplication);
    public string? Instance { get; set; } = GetEnvironmentVariable(VostokEnvironmentVariables.IdentityInstance);
}