using JetBrains.Annotations;

namespace Vostok.Extensions.Hosting.Components.Hercules;

[PublicAPI]
public sealed class HerculesSinkStreamOptions
{
    public string? DefaultApiKey { get; set; }
}