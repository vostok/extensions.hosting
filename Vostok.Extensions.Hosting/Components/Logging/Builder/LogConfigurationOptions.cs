using JetBrains.Annotations;

using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Logging.Builder;

[PublicAPI]
public sealed class LogConfigurationOptions
{
    private readonly List<Func<ILog, ILog>> _customizations = [];
    
    public void CustomizeFinalLog(Func<ILog, ILog> customization) =>
        _customizations.Add(customization);

    internal ILog ApplyLogCustomizations(ILog log)
    {
        foreach (var customization in _customizations)
            log = customization(log);

        return log;
    }
}