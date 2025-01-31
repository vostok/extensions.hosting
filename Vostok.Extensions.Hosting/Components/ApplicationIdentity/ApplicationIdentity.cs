using Microsoft.Extensions.Options;

using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ApplicationIdentity;

internal sealed class ApplicationIdentity : IVostokApplicationIdentity
{
    public string Project { get; }
    public string? Subproject { get; }
    public string Environment { get; }
    public string Application { get; }
    public string Instance { get; }

    public ApplicationIdentity(IOptions<ApplicationIdentityOptions> options)
    {
        var identityOptions = options.Value;

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