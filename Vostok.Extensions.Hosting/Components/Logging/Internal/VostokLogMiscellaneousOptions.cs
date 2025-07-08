using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Logging.Internal;

internal delegate ILog LogCustomization(ILog log, IServiceProvider serviceProvider);

internal sealed class VostokLogMiscellaneousOptions
{
    public List<LogCustomization> Customizations { get; } = [];
    public Func<LogLevel>? MinLevelProvider { get; set; }
}