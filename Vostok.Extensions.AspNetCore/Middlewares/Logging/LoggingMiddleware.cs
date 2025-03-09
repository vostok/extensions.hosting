using System.Diagnostics;
using System.Text;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using Vostok.Clusterclient.Core.Model;
using Vostok.Commons.Formatting;
using Vostok.Commons.Time;
using Vostok.Context;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Abstractions.Values;
using Vostok.Tracing.Abstractions;

namespace Vostok.Extensions.AspNetCore.Middlewares.Logging;

/// <summary>
/// Logs incoming requests and outgoing responses.
/// </summary>
[PublicAPI]
public sealed class LoggingMiddleware(RequestDelegate next, IOptions<LoggingMiddlewareOptions> options, ILog log)
{
    private const int StringBuilderCapacity = 256;

    private readonly LoggingMiddlewareOptions _options = options.Value;
    private readonly ILog _log = log.ForContext<LoggingMiddleware>();

    public async Task InvokeAsync(HttpContext context)
    {
        if (_options.LogRequests)
            LogRequest(context.Request);

        var watch = Stopwatch.StartNew();

        if (_options.LogResponseCompletion)
        {
            var tracingContext = FlowingContext.Globals.Get<TraceContext>();
            var operationContext = FlowingContext.Globals.Get<OperationContextValue>();

            context.Response.OnCompleted(
                () =>
                {
                    using (FlowingContext.Globals.Use(tracingContext))
                    using (FlowingContext.Globals.Use(operationContext))
                    {
                        LogResponseCompleted(watch.Elapsed);
                    }

                    return Task.CompletedTask;
                });
        }

        await next(context);

        if (_options.LogResponses && IsLoggingStatusCode(context.Response))
            LogResponse(context.Request, context.Response, watch.Elapsed);
    }

    private void LogRequest(HttpRequest request)
    {
        var requestInfo = FlowingContext.Globals.Get<IRequestInfo>();
        var builder = StringBuilderCache.Acquire(StringBuilderCapacity);

        var addClientIdentity = requestInfo?.ClientApplicationIdentity != null;
        var addBodySize = request.ContentLength > 0L;
        var addHeaders = _options.LogRequestHeaders.IsEnabledForRequest(request);

        var parametersCount = 3 + (addClientIdentity ? 1 : 0) + (addBodySize ? 1 : 0) + (addHeaders ? 1 : 0);
        var parameters = new object[parametersCount];
        var parametersIndex = 0;

        AppendSegment(builder, parameters, "Received request '{Request}' from",
            FormatPath(builder, request, _options.LogQueryString), ref parametersIndex);

        if (addClientIdentity)
            AppendSegment(builder, parameters, " '{ClientIdentity}' at", requestInfo!.ClientApplicationIdentity!, ref parametersIndex);

        AppendSegment(builder, parameters, " '{RequestConnection}'", GetClientConnectionInfo(request), ref parametersIndex);
        AppendSegment(builder, parameters, " with timeout = {Timeout}", requestInfo?.Timeout.ToPrettyString() ?? "unknown", ref parametersIndex);

        builder.Append('.');

        if (addBodySize)
            AppendSegment(builder, parameters, " Body size = {BodySize}.", request.ContentLength!, ref parametersIndex);

        if (addHeaders)
            AppendSegment(builder, parameters, " Request headers: {RequestHeaders}", FormatHeaders(builder, request.Headers, _options.LogRequestHeaders), ref parametersIndex);

        _log.Info(builder.ToString(), parameters);

        StringBuilderCache.Release(builder);
    }

    private void LogResponse(HttpRequest request, HttpResponse response, TimeSpan elapsed)
    {
        var builder = StringBuilderCache.Acquire(StringBuilderCapacity);

        var addBodySize = response.ContentLength > 0;
        var addHeaders = _options.LogResponseHeaders.IsEnabledForRequest(request);

        builder.Append("Response code = {ResponseCode:D} ('{ResponseCode}'). Time = {ElapsedTime}.");

        if (addBodySize)
            builder.Append(" Body size = {BodySize}.");

        if (addHeaders)
            builder.Append(" Response headers: {ResponseHeaders}");

        var logEvent = new LogEvent(LogLevel.Info, PreciseDateTime.Now, builder.ToString())
                       .WithProperty("Request", FormatPath(builder, request, _options.LogQueryString))
                       .WithProperty("ResponseCode", (ResponseCode)response.StatusCode)
                       .WithProperty("ElapsedTime", elapsed.ToPrettyString())
                       .WithProperty("ElapsedTimeMs", elapsed.TotalMilliseconds);

        if (addBodySize)
            logEvent = logEvent.WithProperty("BodySize", response.ContentLength);

        if (addHeaders)
            logEvent = logEvent.WithProperty("ResponseHeaders", FormatHeaders(builder, response.Headers, _options.LogResponseHeaders));

        _log.Log(logEvent);

        StringBuilderCache.Release(builder);
    }

    private bool IsLoggingStatusCode(HttpResponse response)
    {
        if (_options.LogResponseStatusCodes == null || _options.LogResponseStatusCodes.Length == 0)
            return true;

        return _options.LogResponseStatusCodes.Contains(response.StatusCode);
    }

    private void LogResponseCompleted(TimeSpan elapsed)
        => _log.Info("Response has completed in {ElapsedTime}.",
            new
            {
                ElapsedTime = elapsed.ToPrettyString(),
                ElapsedTimeMs = elapsed.TotalMilliseconds
            });

    private static void AppendSegment(StringBuilder builder,
        object[] parameters,
        string templateSegment,
        object parameter,
        ref int parameterIndex)
    {
        builder.Append(templateSegment);

        parameters[parameterIndex++] = parameter;
    }

    private static string GetClientConnectionInfo(HttpRequest request)
    {
        var connection = request.HttpContext.Connection;
        return $"{connection.RemoteIpAddress}:{connection.RemotePort}";
    }

    private static string FormatPath(StringBuilder builder,
        HttpRequest request,
        LoggingCollectionOptions queryOptions)
    {
        return FormatAndRollback(
            builder,
            b =>
            {
                b.Append(request.Method);
                b.Append(" ");
                b.Append(request.Path);

                if (queryOptions.IsEnabledForRequest(request))
                {
                    if (queryOptions.IsEnabledForAllKeys())
                    {
                        b.Append(request.QueryString);
                    }
                    else
                    {
                        var writtenFirst = false;

                        foreach (var pair in request.Query.Where(kvp => queryOptions.IsEnabledForKey(kvp.Key)))
                        {
                            if (!writtenFirst)
                            {
                                b.Append('?');
                                writtenFirst = true;
                            }

                            b.Append($"{pair.Key}={pair.Value}");
                        }
                    }
                }
            });
    }

    private static string FormatHeaders(StringBuilder builder, IHeaderDictionary headers, LoggingCollectionOptions options)
    {
        return FormatAndRollback(
            builder,
            b =>
            {
                foreach (var pair in headers)
                {
                    if (!options.IsEnabledForKey(pair.Key))
                        continue;

                    b.AppendLine();
                    b.Append('\t');
                    b.Append(pair.Key);
                    b.Append(": ");
                    b.Append(pair.Value);
                }
            });
    }

    private static string FormatAndRollback(StringBuilder builder, Action<StringBuilder> format)
    {
        var positionBefore = builder.Length;

        format(builder);

        var result = builder.ToString(positionBefore, builder.Length - positionBefore);

        builder.Length = positionBefore;

        return result;
    }
}