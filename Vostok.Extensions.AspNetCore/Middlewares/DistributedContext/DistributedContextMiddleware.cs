using JetBrains.Annotations;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using Vostok.Clusterclient.Context;
using Vostok.Clusterclient.Core.Model;
using Vostok.Context;
using Vostok.Extensions.AspNetCore.Models;

namespace Vostok.Extensions.AspNetCore.Middlewares.DistributedContext;

/// <summary>
/// Restores distributed context from request headers and ensures request priority is in <see cref="FlowingContext"/>.
/// </summary>
[PublicAPI]
public sealed class DistributedContextMiddleware(RequestDelegate next, IOptions<DistributedContextSettings> settings)
{
    static DistributedContextMiddleware()
        => FlowingContext.Configuration.RegisterDistributedGlobal(DistributedContextConstants.RequestPriorityGlobalName, new RequestPrioritySerializer());

    public async Task InvokeAsync(HttpContext context)
    {
        FlowingContext.RestoreDistributedProperties(context.Request.Headers[HeaderNames.ContextProperties]!);
        FlowingContext.RestoreDistributedGlobals(context.Request.Headers[HeaderNames.ContextGlobals]!);

        if (FlowingContext.Globals.Get<RequestPriority?>() == null)
        {
            var priority = FlowingContext.Globals.Get<IRequestInfo>()?.Priority ?? RequestPriority.Ordinary;

            FlowingContext.Globals.Set<RequestPriority?>(priority);
            FlowingContext.Globals.Set(priority);
        }

        foreach (var action in settings.Value.AdditionalActions)
            action(context.Request);

        await next(context);
    }
}