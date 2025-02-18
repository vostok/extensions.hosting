using JetBrains.Annotations;

using Vostok.Logging.Abstractions;
using Vostok.Logging.Console;

namespace Vostok.Extensions.Hosting.Components.Logging;

[PublicAPI]
public sealed class ConsoleLogOptions
{
    private readonly List<Action<ConsoleLogSettings>> _customizations = [];

    public bool Enabled { get; set; } = true;
    public bool Synchronous { get; set; } = false;

    public Func<LogLevel>? MinLevelProvider { get; set; }

    public void CustomizeSettings(Action<ConsoleLogSettings> customization)
        => _customizations.Add(customization);

    internal void ApplyCustomizations(ConsoleLogSettings settings) =>
        _customizations.ForEach(customization => customization(settings));
}