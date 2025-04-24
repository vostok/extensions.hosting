using System.Text;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using Vostok.Commons.Environment;

namespace Vostok.Extensions.AspNetCore.Middlewares.PingApi;

/// <summary>
/// Handles diagnostic <c>/_status/ping</c> and <c>/_status/version</c> requests.
/// </summary>
[PublicAPI]
public sealed class PingApiMiddleware(RequestDelegate next, IOptions<PingApiMiddlewareOptions> options)
{
    internal const string PingPath = "/_status/ping";
    internal const string VersionPath = "/_status/version";

    private readonly string _commitHash =
        options.Value.CommitHashProvider?.Invoke() ?? AssemblyCommitHashExtractor.ExtractFromEntryAssembly();

    public Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;

        switch (request.Path.Value)
        {
            case PingPath:
                if (!HttpMethods.IsGet(request.Method))
                    break;

                return HandlePingRequest(context);

            case VersionPath:
                if (!HttpMethods.IsGet(request.Method))
                    break;

                return HandleVersionRequest(context);
        }

        return next.Invoke(context);
    }

    private Task HandlePingRequest(HttpContext context) =>
        HandleRequest(context, "{" + $"\"Status\":\"{GetHealthStatus()}\"" + "}");

    private Task HandleVersionRequest(HttpContext context) =>
        HandleRequest(context, "{" + $"\"CommitHash\":\"{_commitHash}\"" + "}");

    private static Task HandleRequest(HttpContext context, string text)
    {
        context.Response.StatusCode = 200;
        context.Response.ContentType = "application/json";

        var body = Encoding.UTF8.GetBytes(text);
        context.Response.ContentLength = body.Length;
        return context.Response.Body.WriteAsync(body, 0, body.Length);
    }

    private string GetHealthStatus()
    {
        var optionsValue = options.Value;

        var isInitialized = optionsValue.InitializationCheck?.Invoke() ?? true;
        if (isInitialized)
            return optionsValue.HealthCheck?.Invoke() ?? true ? "Ok" : "Warn";

        return "Init";
    }
}