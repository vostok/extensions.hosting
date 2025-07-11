using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Logging.Internal;

internal sealed class VostokLogMiscellaneousOptions
{
    public List<Func<ILog, IServiceProvider, ILog>> Customizations { get; } = [];
    public Func<LogLevel>? MinLevelProvider { get; set; }
}