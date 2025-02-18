using JetBrains.Annotations;

using Vostok.Logging.Abstractions;
using Vostok.Logging.File.Configuration;

namespace Vostok.Extensions.Hosting.Components.Logging;

[PublicAPI]
public sealed class FileLogOptions
{
    private readonly List<Action<FileLogSettings>> _customizations = [];

    public bool Enabled { get; set; } = true;

    public Func<LogLevel>? MinLevelProvider { get; set; }

    public void CustomizeSettings(Action<FileLogSettings> customization)
        => _customizations.Add(customization);

    internal void ApplyCustomizations(FileLogSettings settings) =>
        _customizations.ForEach(customization => customization(settings));
}