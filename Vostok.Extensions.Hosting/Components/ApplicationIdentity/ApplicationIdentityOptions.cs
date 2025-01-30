using static System.Environment;

namespace Vostok.Extensions.Hosting.Components.ApplicationIdentity;

public sealed class ApplicationIdentityOptions
{
    private string? _project = GetEnvironmentVariable(VostokEnvironmentVariables.IdentityProject);
    private string? _subproject = GetEnvironmentVariable(VostokEnvironmentVariables.IdentitySubproject);
    private string? _environment = GetEnvironmentVariable(VostokEnvironmentVariables.IdentityEnvironment);
    private string? _application = GetEnvironmentVariable(VostokEnvironmentVariables.IdentityApplication);
    private string? _instance = GetEnvironmentVariable(VostokEnvironmentVariables.IdentityInstance);

    public string? Project
    {
        get => _project;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _project = value;
        }
    }

    public string? Subproject
    {
        get => _subproject;
        set => _subproject = value;
    }

    public string? Environment
    {
        get => _environment;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _environment = value;
        }
    }

    public string? Application
    {
        get => _application;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _application = value;
        }
    }

    public string? Instance
    {
        get => _instance;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _instance = value;
        }
    }
}