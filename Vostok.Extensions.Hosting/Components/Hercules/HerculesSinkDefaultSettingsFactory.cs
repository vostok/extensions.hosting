using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Clusterclient.Core.Model;
using Vostok.Clusterclient.Tracing;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.Hercules.Client;
using Vostok.Hosting.Abstractions;
using Vostok.Tracing.Abstractions;
using Vostok.Tracing.Extensions.Http;

namespace Vostok.Extensions.Hosting.Components.Hercules;

internal sealed class HerculesSinkDefaultSettingsFactory(
    IEnumerable<IConfigureOptions<HerculesSinkSettings>> setups,
    IEnumerable<IPostConfigureOptions<HerculesSinkSettings>> postConfigures,
    IEnumerable<IValidateOptions<HerculesSinkSettings>> validations,
    IServiceProvider provider) : OptionsFactory<HerculesSinkSettings>(setups, postConfigures, validations)
{
    protected override HerculesSinkSettings CreateInstance(string _)
    {
        var miscOptions = provider.GetRequiredService<IOptions<HerculesSinkMiscellaneousOptions>>().Value;
        var clusterProvider = miscOptions.ClusterProvider?.Invoke(provider);
        if (clusterProvider is null)
            throw new InvalidOperationException("Unconfigured topology");

        var optionsMonitor = provider.GetRequiredService<IOptionsMonitor<HerculesSinkAuthOptions>>();

        var tracer = provider.GetRequiredService<ITracer>();
        var applicationIdentity = provider.GetRequiredService<IVostokApplicationIdentity>();

        // Note(kungurtsev): allow null api key provider, streams can be configured later.
        return new HerculesSinkSettings(clusterProvider, () => optionsMonitor.CurrentValue.ApiKey)
        {
            AdditionalSetup = setup =>
            {
                // note (ponomaryovigor): Do not use ServiceNameProvider.cs because of circular dependency between sink and beacon.
                setup.ClientApplicationName = applicationIdentity.FormatServiceName();

                setup.SetupDistributedTracing(new TracingConfiguration(tracer)
                {
                    SetAdditionalRequestDetails = SetStreamName
                });
            }
        };
    }

    private static void SetStreamName(IHttpRequestSpanBuilder spanBuilder, Request request)
    {
        if (request.TryGetQueryParameter("stream", out var stream))
            spanBuilder.SetAnnotation("http.request.stream", stream);
    }
}