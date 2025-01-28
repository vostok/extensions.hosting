using Vostok.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ApplicationIdentity;

internal sealed class VostokApplicationIdentity : IVostokApplicationIdentity
{
    public string Project { get; }
    public string? Subproject { get; }
    public string Environment { get; }
    public string Application { get; }
    public string Instance { get; }

    public VostokApplicationIdentity(
        string project,
        string? subproject,
        string environment,
        string application,
        string instance)
    {
        var missingFields = new List<string>();

        if (string.IsNullOrWhiteSpace(project))
            missingFields.Add(nameof(project));
        if (string.IsNullOrWhiteSpace(subproject))
            subproject = null;
        if (string.IsNullOrWhiteSpace(environment))
            missingFields.Add(nameof(environment));
        if (string.IsNullOrWhiteSpace(application))
            missingFields.Add(nameof(application));
        if (string.IsNullOrWhiteSpace(instance))
            missingFields.Add(nameof(instance));

        if (missingFields.Count != 0)
            throw new ArgumentException($"Some of the Vostok application identity required fields have not been specified: '{string.Join(", ", missingFields)}'.");

        Project = project;
        Subproject = subproject;
        Environment = environment;
        Application = application;
        Instance = instance;
    }

    public override string ToString()
        => $"Project = {Project}; Subproject = {Subproject ?? "N/A"}; Environment = {Environment}; Application = {Application}; Instance = {Instance}";
}