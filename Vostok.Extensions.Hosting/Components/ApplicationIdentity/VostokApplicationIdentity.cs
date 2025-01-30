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

    public VostokApplicationIdentity(IOptions<ApplicationIdentityOptions> options)
    {
        var identityOptions = options.Value;

        var missingFields = new List<string>();

        if (string.IsNullOrWhiteSpace(identityOptions.Project))
            missingFields.Add(nameof(Project));
        if (string.IsNullOrWhiteSpace(identityOptions.Environment))
            missingFields.Add(nameof(Environment));
        if (string.IsNullOrWhiteSpace(identityOptions.Application))
            missingFields.Add(nameof(Application));
        if (string.IsNullOrWhiteSpace(identityOptions.Instance))
            missingFields.Add(nameof(Instance));

        if (missingFields.Count != 0)
            throw new ArgumentException(
                $"Some of the Vostok application identity required fields have not been specified: '{string.Join(", ", missingFields)}'.");

        Project = identityOptions.Project!;
        Subproject = identityOptions.Subproject;
        Environment = identityOptions.Environment!;
        Application = identityOptions.Application!;
        Instance = identityOptions.Instance!;
    }

    public override string ToString()
        => $"Project = {Project}; Subproject = {Subproject ?? "N/A"}; Environment = {Environment}; Application = {Application}; Instance = {Instance}";
}