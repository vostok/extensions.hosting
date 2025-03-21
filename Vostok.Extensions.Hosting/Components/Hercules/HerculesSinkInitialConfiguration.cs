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

internal sealed class HerculesSinkInitialConfiguration(IServiceProvider provider)
    : IConfigureOptions<HerculesSinkSettings>
{
    public void Configure(HerculesSinkSettings settings)
    {
        var tracer = provider.GetService<ITracer>();
        var applicationIdentity = provider.GetService<IVostokApplicationIdentity>();

        // Note(kungurtsev): allow null api key provider, streams can be configured later.
        settings.AdditionalSetup += setup =>
        {
            if (applicationIdentity is not null)
                setup.ClientApplicationName = applicationIdentity.FormatServiceName();

            if (tracer is not null)
                setup.SetupDistributedTracing(new TracingConfiguration(tracer)
                {
                    SetAdditionalRequestDetails = SetStreamName
                });
        };
    }

    private static void SetStreamName(IHttpRequestSpanBuilder spanBuilder, Request request)
    {
        if (request.TryGetQueryParameter("stream", out var stream))
            spanBuilder.SetAnnotation("http.request.stream", stream);
    }
}