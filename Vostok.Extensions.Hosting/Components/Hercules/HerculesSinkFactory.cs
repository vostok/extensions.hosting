using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Clusterclient.Core.Model;
using Vostok.Clusterclient.Core.Topology;
using Vostok.Clusterclient.Tracing;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.Hercules.Client;
using Vostok.Hercules.Client.Abstractions;
using Vostok.Hosting.Abstractions;
using Vostok.Logging.Abstractions;
using Vostok.Tracing.Abstractions;
using Vostok.Tracing.Extensions.Http;

namespace Vostok.Extensions.Hosting.Components.Hercules;

internal static class HerculesSinkFactory
{
    public static IHerculesSink Create(IServiceProvider provider)
    {
        var log = provider.GetRequiredService<ILog>();
        var options = provider.GetRequiredService<IOptions<HerculesSinkConfigurationOptions>>().Value;

        if (!options.Enabled)
        {
            log.LogDisabled("HerculesSink", "explicit disable");
            return new DevNullHerculesSink();
        }

        if (!options.VerboseLogging)
            log = log.WithMinimumLevel(LogLevel.Warn);

        var clusterProvider = options.ClusterProviderBuilder?.Build(provider);
        if (clusterProvider is null)
        {
            log.LogDisabled("HerculesSink", "unconfigured cluster provider");
            return new DevNullHerculesSink();
        }

        var herculesSinkSettings = CreateSettings(options, clusterProvider, provider);

        return new HerculesSink(herculesSinkSettings, log);
    }

    private static HerculesSinkSettings CreateSettings(
        HerculesSinkConfigurationOptions options,
        IClusterProvider clusterProvider,
        IServiceProvider serviceProvider)
    {
        var tracer = serviceProvider.GetRequiredService<ITracer>();
        var applicationIdentity = serviceProvider.GetRequiredService<IVostokApplicationIdentity>();

        var formattedServiceName = applicationIdentity.FormatServiceName();

        // Note(kungurtsev): allow null api key provider, streams can be configured later.
        var herculesSinkSettings = new HerculesSinkSettings(clusterProvider, options.ApiKeyProvider)
        {
            AdditionalSetup = setup =>
            {
                setup.ClientApplicationName = formattedServiceName;
                setup.SetupDistributedTracing(new TracingConfiguration(tracer)
                {
                    SetAdditionalRequestDetails = SetStreamName
                });
            }
        };

        options.ApplyCustomizations(herculesSinkSettings);

        return herculesSinkSettings;
    }
    
    private static void SetStreamName(IHttpRequestSpanBuilder spanBuilder, Request request)
    {
        if (request.TryGetQueryParameter("stream", out var stream))
            spanBuilder.SetAnnotation("http.request.stream", stream);
    }
}