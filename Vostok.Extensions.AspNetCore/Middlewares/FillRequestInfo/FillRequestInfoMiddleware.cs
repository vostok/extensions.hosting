using System.Net;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using Vostok.Clusterclient.Core.Model;
using Vostok.Commons.Helpers;
using Vostok.Context;
using Vostok.Extensions.AspNetCore.Diagnostics;
using Vostok.Extensions.AspNetCore.Models;

namespace Vostok.Extensions.AspNetCore.Middlewares.FillRequestInfo;

/// <summary>
/// Populates an <see cref="IRequestInfo"/> object from request properties and stores as a <see cref="FlowingContext"/> global for the lifetime of the request.
/// </summary>
[PublicAPI]
public class FillRequestInfoMiddleware(
    RequestDelegate next,
    IOptions<FillRequestInfoSettings> options,
    IRequestTracker tracker)
{
    private readonly FillRequestInfoSettings _options = options.Value;

    public async Task InvokeAsync(HttpContext context)
    {
        IRequestInfo requestInfo = new RequestInfo(
            GetTimeout(context.Request),
            GetPriority(context.Request),
            GetClientApplicationIdentity(context.Request),
            context.Request.HttpContext.Connection.RemoteIpAddress ?? IPAddress.None);

        FlowingContext.Globals.Set(requestInfo);

        context.Response.RegisterForDispose(tracker.Track(context, requestInfo));

        await next(context);
    }

    private static TResult? ObtainFromProviders<TResult>(HttpRequest request, IEnumerable<Func<HttpRequest, TResult>> providers)
        => providers.Select(provider => provider(request)).FirstOrDefault();

    private TimeSpan GetTimeout(HttpRequest request)
    {
#pragma warning disable CS8604 // Possible null reference argument.
        if (NumericTypeParser<double>.TryParse(request.Headers[HeaderNames.RequestTimeout], out var seconds))
#pragma warning restore CS8604 // Possible null reference argument.
            return TimeSpan.FromSeconds(seconds);

        return ObtainFromProviders(request, _options.AdditionalTimeoutProviders) ?? _options.DefaultTimeoutProvider(request);
    }

    private RequestPriority GetPriority(HttpRequest request)
    {
        if (Enum.TryParse(request.Headers[HeaderNames.RequestPriority], true, out RequestPriority priority))
            return priority;

        return ObtainFromProviders(request, _options.AdditionalPriorityProviders) ?? _options.DefaultPriorityProvider(request);
    }

    private string? GetClientApplicationIdentity(HttpRequest request)
    {
        var clientApplicationIdentity = request.Headers[HeaderNames.ApplicationIdentity].ToString();
        if (!string.IsNullOrEmpty(clientApplicationIdentity))
            return clientApplicationIdentity;

        return ObtainFromProviders(request, _options.AdditionalClientIdentityProviders);
    }
}