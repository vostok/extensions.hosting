using JetBrains.Annotations;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Extensions.AspNetCore.Middlewares;
using Vostok.Extensions.AspNetCore.Middlewares.DatacentersAwareness;
using Vostok.Extensions.AspNetCore.Middlewares.DiagnosticApi;
using Vostok.Extensions.AspNetCore.Middlewares.DistributedContext;
using Vostok.Extensions.AspNetCore.Middlewares.FillRequestInfo;
using Vostok.Extensions.AspNetCore.Middlewares.HttpContextTweak;
using Vostok.Extensions.AspNetCore.Middlewares.Logging;
using Vostok.Extensions.AspNetCore.Middlewares.PingApi;
using Vostok.Extensions.AspNetCore.Middlewares.Throttling;
using Vostok.Extensions.AspNetCore.Middlewares.UnhandledException;
using Vostok.ServiceDiscovery.Abstractions;

namespace Vostok.Extensions.AspNetCore;

/// <summary>
/// <para>Adds Vostok middlewares to the specified application builder.</para>
/// </summary>
[PublicAPI]
public static class VostokMiddlewaresApplicationBuilderExtensions
{
    private const string Slash = "/";

    /// <inheritdoc cref="VostokMiddlewaresApplicationBuilderExtensions"/>
    public static IApplicationBuilder UseVostokMiddlewares(this IApplicationBuilder applicationBuilder)
    {
        var settings = applicationBuilder.ApplicationServices.GetRequiredService<IOptions<VostokMiddlewaresOptions>>().Value;
        settings.MiddlewaresAdded = true;

        var middlewares = new List<Type>();

        applicationBuilder.UseBeaconPathBase();

        Add<HttpContextTweakMiddleware>();
        Add<FillRequestInfoMiddleware>();
        Add<DistributedContextMiddleware>();
        Add<ThrottlingMiddleware>();
        Add<LoggingMiddleware>();
        Add<DatacenterAwarenessMiddleware>();
        Add<UnhandledExceptionMiddleware>();
        Add<PingApiMiddleware>();
        Add<DiagnosticApiMiddleware>();

        foreach (var middleware in middlewares)
            applicationBuilder.UseMiddleware(middleware);

        return applicationBuilder;

        void Add<TMiddleware>()
        {
            if (settings.PreVostokMiddlewares.TryGetValue(typeof(TMiddleware), out var injected))
                middlewares.AddRange(injected);

            if (settings.IsEnabled<TMiddleware>())
            {
                middlewares.Add(typeof(TMiddleware));
            }
        }
    }

    private static void UseBeaconPathBase(this IApplicationBuilder applicationBuilder)
    {
        var serviceBeacon = applicationBuilder.ApplicationServices.GetService<IServiceBeacon>();
        if (serviceBeacon is null)
            return;

        if (!serviceBeacon.ReplicaInfo.TryGetUrl(out var url) is false)
            return;

        var urlPath = url.AbsolutePath;
        if (string.IsNullOrEmpty(urlPath) || urlPath == Slash)
            return;

        applicationBuilder.UsePathBase(urlPath);
    }
}