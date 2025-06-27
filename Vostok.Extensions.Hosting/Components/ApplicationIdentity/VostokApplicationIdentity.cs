using Microsoft.Extensions.Options;

using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ApplicationIdentity;

internal sealed class VostokApplicationIdentity : IVostokApplicationIdentity
{
    public string Project { get; }
    public string? Subproject { get; }
    public string Environment { get; }
    public string Application { get; }
    public string Instance { get; }

    // note(ponomaryovigor):
    // Despite identity is a cold options, we are using IOptionsMonitor here because it's used by Startup validator and already warmed up.
    // So we can rid of extra validation check here.
    public VostokApplicationIdentity(IOptionsMonitor<ApplicationIdentityOptions> options)
    {
        var identityOptions = options.CurrentValue;

        ArgumentException.ThrowIfNullOrWhiteSpace(identityOptions.Project);
        ArgumentException.ThrowIfNullOrWhiteSpace(identityOptions.Environment);
        ArgumentException.ThrowIfNullOrWhiteSpace(identityOptions.Application);
        ArgumentException.ThrowIfNullOrWhiteSpace(identityOptions.Instance);

        Project = identityOptions.Project;
        Subproject = identityOptions.Subproject;
        Environment = identityOptions.Environment;
        Application = identityOptions.Application;
        Instance = identityOptions.Instance;
    }

    public override string ToString()
        => $"Project = {Project}; Subproject = {Subproject ?? "N/A"}; Environment = {Environment}; Application = {Application}; Instance = {Instance}";
}