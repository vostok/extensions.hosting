using JetBrains.Annotations;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Vostok.Extensions.AspNetCore.Helpers;

namespace Vostok.Extensions.AspNetCore.Middlewares.HttpContextTweak;

/// <summary>
/// Applies performance tweaks to <see cref="HttpContext"/>.
/// </summary>
[PublicAPI]
public sealed class HttpContextTweakMiddleware(
    RequestDelegate next,
    IOptions<HttpContextTweakSettings> options,
    ILogger<HttpContextTweakMiddleware> log)
{
    public Task InvokeAsync(HttpContext context)
    {
        var optionsValue = options.Value;

        try
        {
            if (optionsValue.EnableResponseWriteCallSizeLimit && IsOlderThanHttp2(context))
                context.Response.Body = new ResponseStreamWrapper(context.Response.Body, optionsValue.MaxResponseWriteCallSize);
        }
        catch (Exception error)
        {
            log.LogError(error, "Error while tweaking HttpContext.");
        }

        return next(context);
    }

    private static bool IsOlderThanHttp2(HttpContext context)
    {
        var protocol = context.Request.Protocol;

        return HttpProtocol.IsHttp11(protocol) || HttpProtocol.IsHttp10(protocol);
    }
}