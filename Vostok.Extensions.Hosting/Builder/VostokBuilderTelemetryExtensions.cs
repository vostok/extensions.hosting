using System.Diagnostics;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Components.Logging;
using Vostok.Extensions.Hosting.Components.Logging.Builder;
using Vostok.Extensions.Hosting.Components.Metrics;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.Hosting.Abstractions;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Microsoft;
using Vostok.Metrics;
using Vostok.Tracing.Abstractions;
using Vostok.Tracing.Diagnostics;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class VostokBuilderTelemetryExtensions
{
    public const string ActualLogKey = "_ActualLog";

    public static IVostokBuilder WithLogging(this IVostokBuilder builder, Action<IVostokLogBuilder> configure)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor =
                ServiceDescriptor.Singleton<ILoggerProvider, VostokLoggerProvider>(VostokLogProviderFactory.Create);
            services.TryAddEnumerable(descriptor);

            var vostokLogBuilder = new VostokLogBuilder(services);
            configure(vostokLogBuilder);
        });

        return builder;
    }

    public static IVostokBuilder WithMetrics(
        this IVostokBuilder builder,
        Action<IVostokMetricsBuilder>? configure = null)
    {
        builder.ConfigureServices(services =>
        {
            services.AddMetrics();

            var metricsBuilder = new VostokMetricsBuilder(services);
            configure?.Invoke(metricsBuilder);
        });

        return builder;
    }

    internal static void WithVostokTracerAdapter(this IVostokBuilder builder)
    {
        var settings = new ActivitySourceTracerSettings
        {
            ActivitySource = new ActivitySource(TracingConstants.VostokTracerActivitySourceName)
        };

        var activitySourceTracer = new ActivitySourceTracer(settings);
        TracerProvider.Configure(activitySourceTracer, true);

        builder.ConfigureServices(services => services.TryAddSingleton<ITracer>(activitySourceTracer));
    }

    internal static void WithVostokLogAdapter(this IVostokBuilder builder)
    {
        var substitutableLog = InitializationLogProvider.Get();
        LogProvider.Configure(substitutableLog, true);

        builder.ConfigureServices(services =>
        {
            services.TryAddSingleton<ILog>(substitutableLog);
            services.TryAddKeyedSingleton<ILog>(ActualLogKey, (provider, _) =>
            {
                var microsoftLogger = provider.GetRequiredService<ILoggerFactory>()
                                              .CreateLogger(string.Empty);

                return new MicrosoftLog(microsoftLogger, new MicrosoftLogSettings());
            });
        });
    }

    private static void AddMetrics(this IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IOptionsFactory<MetricContextConfig>, MetricContextDefaultConfigFactory>());
        services.TryAddSingleton(RootMetricsContextFactory.Create);
        services.TryAddSingleton<IVostokApplicationMetrics, VostokApplicationMetrics>();
    }
}