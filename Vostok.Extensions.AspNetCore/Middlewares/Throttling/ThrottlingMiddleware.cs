using JetBrains.Annotations;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Vostok.Clusterclient.Core.Model;
using Vostok.Commons.Helpers.Url;
using Vostok.Commons.Time;
using Vostok.Context;
using Vostok.Throttling;

namespace Vostok.Extensions.AspNetCore.Middlewares.Throttling;

/// <summary>
/// Limits request parallelism. See <see cref="IThrottlingProvider"/> for more info.
/// </summary>
[PublicAPI]
public sealed class ThrottlingMiddleware(
    RequestDelegate next,
    IOptions<ThrottlingMiddlewareOptions> options,
    IThrottlingProvider provider,
    ILogger<ThrottlingMiddleware> logger)
{
    private const long LargeRequestBodySize = 256 * 1024;
    private static readonly TimeSpan LongThrottlingWaitTime = TimeSpan.FromMilliseconds(500);

    private readonly ThrottlingMiddlewareOptions _options = options.Value;

    public async Task InvokeAsync(HttpContext context)
    {
        if (IsDisabled(context))
        {
            await next(context);
            return;
        }

        var info = FlowingContext.Globals.Get<IRequestInfo>();
        var properties = BuildThrottlingProperties(context, info);

        using var result = await provider.ThrottleAsync(properties, info?.RemainingTimeout);
        if (context.RequestAborted.IsCancellationRequested)
        {
            LogConnectionAlreadyAborted(context, info);
            context.Response.StatusCode = (int)ResponseCode.Canceled;
            return;
        }

        if (result.Status == ThrottlingStatus.Passed)
        {
            if (result.WaitTime >= LongThrottlingWaitTime)
                LogWaitTime(context, info, result);

            await next(context);

            return;
        }

        LogFailure(context, info, result);

        if (ShouldAbortConnection(context, result))
        {
            LogAbortingConnection();
            context.Abort();
        }
        else
        {
            context.Response.StatusCode = _options.RejectionResponseCode;
            context.Response.Headers.ContentLength = 0L;
        }
    }

    private static bool ShouldAbortConnection(HttpContext context, IThrottlingResult result)
        => result.Status == ThrottlingStatus.RejectedDueToDeadline ||
           context.Request.ContentLength > LargeRequestBodySize ||
           context.Request.Headers[HeaderNames.TransferEncoding] == "chunked";

    private static string GetClientConnectionInfo(HttpContext context)
    {
        try
        {
            return $"{context.Connection.RemoteIpAddress}:{context.Connection.RemotePort}";
        }
        catch
        {
            return "unknown endpoint";
        }
    }

    private bool IsDisabled(HttpContext context)
    {
        if (_options.DisableForWebSockets && context.WebSockets.IsWebSocketRequest)
            return true;

        return _options.Enabled != null && !_options.Enabled(context);
    }

    private IReadOnlyDictionary<string, string> BuildThrottlingProperties(HttpContext context, IRequestInfo? info)
    {
        var builder = new ThrottlingPropertiesBuilder();

        if (_options.AddConsumerProperty)
            builder.AddConsumer(info?.ClientApplicationIdentity);

        if (_options.AddPriorityProperty)
            builder.AddPriority(info?.Priority.ToString());

        if (_options.AddMethodProperty)
            builder.AddMethod(context.Request.Method);

        if (_options.AddUrlProperty)
            builder.AddUrl(UrlNormalizer.NormalizePath(context.Request.Path));

        foreach (var additionalProperty in _options.AdditionalProperties)
        {
            var (propertyName, propertyValue) = additionalProperty(context);
            builder.AddProperty(propertyName, propertyValue);
        }

        return builder.Build();
    }

    private void LogWaitTime(HttpContext context, IRequestInfo? info, IThrottlingResult result)
    {
        using (logger.BeginScope(new[]
               {
                   new KeyValuePair<string, object>("ThrottlingWaitTimeMs", result.WaitTime.TotalMilliseconds)
               }))
        {
            logger.LogWarning(
                "Request from '{ClientIdentity}' at {RequestConnection} spent {ThrottlingWaitTime} on throttling.",
                info?.ClientApplicationIdentity ?? "unknown",
                GetClientConnectionInfo(context),
                result.WaitTime.ToPrettyString()
            );
        }
    }

    private void LogFailure(HttpContext context, IRequestInfo? info, IThrottlingResult result)
    {
        const string messageTemplate =
            "Dropping request from '{ClientIdentity}' at {RequestConnection} due to throttling status {ThrottlingStatus}. Rejection reason = '{RejectionReason}'.";

        var args = new object[]
        {
            info?.ClientApplicationIdentity ?? "unknown",
            GetClientConnectionInfo(context),
            result.Status,
            result.RejectionReason ?? "unknown"
        };

        if (result.Status is ThrottlingStatus.RejectedDueToQuota or ThrottlingStatus.RejectedDueToDeadline)
            logger.LogWarning(messageTemplate, args);
        else
            logger.LogError(messageTemplate, args);
    }

    private void LogConnectionAlreadyAborted(HttpContext context, IRequestInfo? info)
        => logger.LogWarning(
            "Request from '{ClientIdentity}' at {RequestConnection} was aborted by client before passing through throttling.",
            info?.ClientApplicationIdentity ?? "unknown",
            GetClientConnectionInfo(context));

    private void LogAbortingConnection()
        => logger.LogInformation("Aborting client connection..");
}