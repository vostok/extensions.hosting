using JetBrains.Annotations;

using Vostok.Logging.Configuration;

namespace Vostok.Extensions.Hosting.Components.Logging;

[PublicAPI]
public sealed class VostokLogRulesOptions
{
    public List<LogConfigurationRule> Rules { get; } = [];
}