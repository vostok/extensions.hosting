using System.Net;
using System.Text;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

using Vostok.Configuration;
using Vostok.Configuration.Printing;
using Vostok.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.AspNetCore.Middlewares.DiagnosticApi;

/// <summary>
/// Handles a wide range of diagnostic info requests under <c>/_diagnostic</c> prefix.
/// </summary>
[PublicAPI]
public sealed class DiagnosticApiMiddleware
{
    private readonly RequestDelegate _next;
    private readonly DiagnosticApiMiddlewareOptions _options;
    private readonly ILogger<DiagnosticApiMiddleware> _logger;
    private readonly IVostokApplicationDiagnostics? _diagnostics;

    private readonly string _pathPrefix;

    public DiagnosticApiMiddleware(
        RequestDelegate next,
        IOptions<DiagnosticApiMiddlewareOptions> options,
        ILogger<DiagnosticApiMiddleware> logger,
        IServiceProvider provider)
    {
        _next = next;
        _options = options.Value;
        _logger = logger;
        _diagnostics = provider.GetService<IVostokApplicationDiagnostics>();

        _pathPrefix = _options.PathPrefix.TrimEnd('/');
    }

    public Task InvokeAsync(HttpContext context)
    {
        if (_diagnostics is null)
            return _next.Invoke(context);

        var diagnosticPath = GetDiagnosticPath(context);
        if (diagnosticPath == null)
            return _next.Invoke(context);

        if (diagnosticPath == string.Empty)
            return HandleListRequest(context, _diagnostics.Info);

        if (DiagnosticEntry.TryParse(diagnosticPath, out var entry) && _diagnostics.Info.TryQuery(entry, out var payload))
            return HandleInfoRequest(context, payload);

        return _next.Invoke(context);
    }

    private Task HandleInfoRequest(HttpContext context, object payload)
    {
        if (!TryAuthorize(context))
            return Task.CompletedTask;

        var printSettings = new PrintSettings {Format = PrintFormat.JSON};
        var responseBody = Encoding.UTF8.GetBytes(ConfigurationPrinter.Print(payload, printSettings));

        context.Response.StatusCode = 200;
        context.Response.ContentLength = responseBody.Length;
        context.Response.ContentType = "application/json";

        DisableResponseCaching(context);

        return context.Response.Body.WriteAsync(responseBody, 0, responseBody.Length);
    }

    private Task HandleListRequest(HttpContext context, IDiagnosticInfo info)
    {
        if (!TryAuthorize(context))
            return Task.CompletedTask;

        var responseBody = Encoding.UTF8.GetBytes(ComposeListPage(context, info));

        context.Response.StatusCode = 200;
        context.Response.ContentLength = responseBody.Length;
        context.Response.ContentType = "text/html";

        DisableResponseCaching(context);

        return context.Response.Body.WriteAsync(responseBody, 0, responseBody.Length);
    }

    private string? GetDiagnosticPath(HttpContext context)
    {
        var requestPath = context.Request.Path.Value;

        if (requestPath is null)
            return null;

        if (!requestPath.StartsWith(_pathPrefix, StringComparison.OrdinalIgnoreCase))
            return null;

        return requestPath.Substring(_pathPrefix.Length).Trim('/');
    }

    private string ComposeListPage(HttpContext context, IDiagnosticInfo info)
    {
        var builder = new StringBuilder();

        foreach (var group in info.ListAll()
                                  .GroupBy(entry => entry.Component, StringComparer.OrdinalIgnoreCase)
                                  .OrderBy(group => group.Key))
        {
            builder.AppendLine($"<h2>{group.Key}</h2>");
            builder.AppendLine("<ul>");

            foreach (var entry in group.OrderBy(entry => entry.Name, StringComparer.OrdinalIgnoreCase))
            {
                builder.AppendLine($"<li><a href=\"{CreateInfoUrl(entry)}\">{entry.Name}</a></li>");
            }

            builder.AppendLine("</ul>");
            builder.AppendLine("<br/>");
        }

        return builder.ToString();
    }

    private string CreateInfoUrl(DiagnosticEntry entry) => _pathPrefix.TrimStart('/') + '/' + entry;

    private static void DisableResponseCaching(HttpContext context)
    {
        context.Response.Headers[HeaderNames.CacheControl] = "no-store, no-cache";
        context.Response.Headers[HeaderNames.Pragma] = "no-cache";
        context.Response.Headers[HeaderNames.Expires] = "Thu, 01 Jan 1970 00:00:00 GMT";
    }

    private bool TryAuthorize(HttpContext context)
    {
        if (IsAuthorized(context))
            return true;

        _logger.LogWarning("Unauthorized request to '{Path}' from {Address}.", context.Request.Path, context.Connection.RemoteIpAddress);

        context.Response.StatusCode = 403;
        context.Response.ContentLength = 0;

        return false;
    }

    private bool IsAuthorized(HttpContext context)
    {
        if (_options.AllowOnlyLocalRequests && !IsLocalRequest(context))
            return false;

        foreach (var header in _options.ProhibitedHeaders)
            if (context.Request.Headers.ContainsKey(header))
                return false;

        return true;
    }

    private static bool IsLocalRequest(HttpContext context) =>
        context.Connection.RemoteIpAddress.Equals(context.Connection.LocalIpAddress) ||
        IPAddress.IsLoopback(context.Connection.RemoteIpAddress);
}