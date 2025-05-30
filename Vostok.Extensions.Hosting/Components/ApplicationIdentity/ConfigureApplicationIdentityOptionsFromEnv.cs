using Microsoft.Extensions.Options;

using static System.Environment;

using static Vostok.Extensions.Hosting.VostokEnvironmentVariables;

namespace Vostok.Extensions.Hosting.Components.ApplicationIdentity;

internal sealed class ConfigureApplicationIdentityOptionsFromEnv : IConfigureOptions<ApplicationIdentityOptions>
{
    public void Configure(ApplicationIdentityOptions options)
    {
        if (GetEnvironmentVariable(IdentityProject) is { } project)
            options.Project = project;
        if (GetEnvironmentVariable(IdentitySubproject) is { } subproject)
            options.Subproject = subproject;
        if (GetEnvironmentVariable(IdentityEnvironment) is { } environment)
            options.Environment = environment;
        if (GetEnvironmentVariable(IdentityApplication) is { } application)
            options.Application = application;
        if (GetEnvironmentVariable(IdentityInstance) is { } instance)
            options.Instance = instance;
    }
}