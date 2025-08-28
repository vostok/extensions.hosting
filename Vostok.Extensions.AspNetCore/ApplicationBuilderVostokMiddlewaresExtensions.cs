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
using Vostok.Extensions.Hosting;
using Vostok.ServiceDiscovery.Abstractions;

namespace Vostok.Extensions.AspNetCore;

[PublicAPI]
public static class ApplicationBuilderVostokMiddlewaresExtensions
{
    private const string Slash = "/";

    /// <summary>
    /// Adds Vostok middlewares to the specified <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static IApplicationBuilder UseVostokMiddlewares(this IApplicationBuilder applicationBuilder)
    {
        CheckWebComponentsRegistered(applicationBuilder);

        var options = applicationBuilder.ApplicationServices.GetRequiredService<IOptions<VostokMiddlewaresOptions>>().Value;
        options.MiddlewaresAdded = true;

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

        return applicationBuilder;

        void Add<TMiddleware>()
        {
            if (options.PreVostokMiddlewares.TryGetValue(typeof(TMiddleware), out var injected))
            {
                foreach (var middleware in injected)
                    applicationBuilder.UseMiddleware(middleware);
            }

            if (options.IsEnabled<TMiddleware>())
                applicationBuilder.UseMiddleware<TMiddleware>();
        }
    }

    private static void UseBeaconPathBase(this IApplicationBuilder applicationBuilder)
    {
        var serviceBeacon = applicationBuilder.ApplicationServices.GetService<IServiceBeacon>();

        if (serviceBeacon is null || !serviceBeacon.ReplicaInfo.TryGetUrl(out var url))
            return;

        var urlPath = url.AbsolutePath;
        if (string.IsNullOrEmpty(urlPath) || urlPath is Slash)
            return;

        applicationBuilder.UsePathBase(urlPath);
    }

    private static void CheckWebComponentsRegistered(IApplicationBuilder applicationBuilder)
    {
        var vostokComponents = applicationBuilder.ApplicationServices
                                                 .GetService<VostokBuilderCoreExtensions.RegisteredVostokComponents>();

        if (vostokComponents?.Components.Contains(VostokBuilderWebComponentsExtensions.WebComponentsKey) is false)
            throw new InvalidOperationException($"`{nameof(VostokBuilderWebComponentsExtensions.WithWebComponents)}()` extension should be called on `{nameof(IVostokBuilder)}` before using Vostok middlewares.");
    }
}