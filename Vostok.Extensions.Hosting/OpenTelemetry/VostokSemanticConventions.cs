namespace Vostok.Extensions.Hosting.OpenTelemetry;

internal static class VostokSemanticConventions
{
    public const string AttributeHostNameForTraces = "host.name";
    public const string AttributeHostNameForLogs = "hostName"; // note: ELK doesn't like dots in attribute name 
    public const string AttributeDeploymentEnvironmentName = "deployment.environment.name";
}