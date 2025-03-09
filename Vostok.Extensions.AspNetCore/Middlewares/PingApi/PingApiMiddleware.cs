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
public sealed class PingApiMiddleware(RequestDelegate next, IOptions<PingApiSettings> settings)
{
    private readonly string _commitHash =
        settings.Value.CommitHashProvider?.Invoke() ?? AssemblyCommitHashExtractor.ExtractFromEntryAssembly();

    public Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;

        switch (request.Path.Value)
        {
            case "/_status/ping":
                if (!HttpMethods.IsGet(request.Method))
                    break;

                return HandlePingRequest(context);

            case "/_status/version":
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
        var settingsValue = settings.Value;

        var isInitialized = settingsValue.InitializationCheck?.Invoke() ?? true;
        if (isInitialized)
            return settingsValue.HealthCheck?.Invoke() ?? true ? "Ok" : "Warn";

        return "Init";
    }
}