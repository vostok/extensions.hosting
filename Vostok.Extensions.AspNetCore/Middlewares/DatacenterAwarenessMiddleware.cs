using JetBrains.Annotations;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Vostok.Datacenters;
using Vostok.Extensions.AspNetCore.Builder;

namespace Vostok.Extensions.AspNetCore.Middlewares;

/// <summary>
/// Rejects incoming requests when local datacenter is not active.
/// </summary>
[PublicAPI]
public class DatacenterAwarenessMiddleware(
    RequestDelegate next,
    IOptions<DatacenterAwarenessSettings> options,
    IDatacenters datacenters,
    ILogger<DatacenterAwarenessMiddleware> logger)
{
    private readonly DatacenterAwarenessSettings _options = options.Value;

    public async Task InvokeAsync(HttpContext context)
    {
        if (_options.RejectRequestsWhenDatacenterIsInactive && !datacenters.LocalDatacenterIsActive())
        {
            context.Response.StatusCode = _options.RejectionResponseCode;

            logger.LogWarning("Rejecting request as local datacenter '{Datacenter}' is not active.", datacenters.GetLocalDatacenter());

            return;
        }

        await next(context);
    }
}