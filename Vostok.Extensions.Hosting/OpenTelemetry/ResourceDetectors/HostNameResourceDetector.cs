using OpenTelemetry.Resources;

using Vostok.Commons.Environment;

namespace Vostok.Extensions.Hosting.OpenTelemetry.ResourceDetectors;

internal sealed class HostNameResourceDetector(string hostNameAttribute) : IResourceDetector
{
    public Resource Detect() =>
        new([new KeyValuePair<string, object>(hostNameAttribute, EnvironmentInfo.Host)]);
}