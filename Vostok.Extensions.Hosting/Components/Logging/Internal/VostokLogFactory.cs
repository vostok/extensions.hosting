using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Vostok.Logging.Abstractions;
using Vostok.Logging.Microsoft;
using Vostok.Logging.Tracing;
using Vostok.Tracing.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Logging.Internal;

/// <summary>
/// Logging structure looks like: Vostok log adapter (ILog) -> VostokLoggerProvider (MS logger implementation) -> Actual Vostok log impl (ILog).
/// </summary>
internal static class VostokLogFactory
{
    public const string LogKey = "_LogImplementation";

    public static ILog CreateVostokLogAdapter(IServiceProvider provider)
    {
        var microsoftLogger = provider.GetRequiredService<ILoggerFactory>()
                                      .CreateLogger(string.Empty);

        ILog logAdapter = new MicrosoftLog(microsoftLogger, new MicrosoftLogSettings());

        // note: Options for main ILog registered with default name.
        var options = provider.GetRequiredService<IOptionsSnapshot<VostokLogMiscellaneousOptions>>().Value;
        foreach (var customization in options.Customizations)
            logAdapter = customization(logAdapter, provider);

        if (options.MinLevelProvider is not null)
            logAdapter = logAdapter.WithMinimumLevel(options.MinLevelProvider);

        return logAdapter;
    }
    
    public static VostokLoggerProvider CreateLoggerProvider(IServiceProvider provider)
    {
        var vostokLog = provider.GetRequiredKeyedService<ILog>(LogKey);
        var settings = provider.GetRequiredService<IOptions<VostokLoggerProviderSettings>>().Value;

        return new VostokLoggerProvider(vostokLog, settings);
    }

    public static ILog CreateVostokLogImplementation(IServiceProvider provider)
    {
        var vostokLog = GetVostokLog(provider);
        return vostokLog is SilentLog
            ? vostokLog
            : new InternalDisposableVostokLog(vostokLog);
    }

    private static ILog GetVostokLog(IServiceProvider provider)
    {
        var logRegistrations = provider.GetKeyedServices<LogRegistration>(LogRegistration.Key.All).ToArray();
        if (logRegistrations.Length is 0)
            return new SilentLog();

        var baseLogs = logRegistrations.Select(r => r.Create(provider)).ToArray();
        var compositeLog = new CompositeLog(baseLogs);

        return compositeLog.WithTracingProperties(provider.GetRequiredService<ITracer>());
    }
}