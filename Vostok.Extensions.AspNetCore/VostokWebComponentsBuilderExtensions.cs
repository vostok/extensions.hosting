using JetBrains.Annotations;

using Vostok.Extensions.AspNetCore.Middlewares.DatacentersAwareness;
using Vostok.Extensions.AspNetCore.Middlewares.DiagnosticApi;
using Vostok.Extensions.AspNetCore.Middlewares.DistributedContext;
using Vostok.Extensions.AspNetCore.Middlewares.FillRequestInfo;
using Vostok.Extensions.AspNetCore.Middlewares.HttpContextTweak;
using Vostok.Extensions.AspNetCore.Middlewares.Logging;
using Vostok.Extensions.AspNetCore.Middlewares.PingApi;
using Vostok.Extensions.AspNetCore.Middlewares.Throttling;
using Vostok.Extensions.AspNetCore.Middlewares.UnhandledException;

namespace Vostok.Extensions.AspNetCore;

[PublicAPI]
public static class VostokWebComponentsBuilderExtensions
{
    /// <summary>
    /// <para>Disables the built-in Vostok middleware of given type <typeparamref name="TMiddleware"></typeparamref>.</para>
    /// <para>Current list of Vostok middlewares:</para>
    /// <list type="bullet">
    ///     <item><description><see cref="HttpContextTweakMiddleware"/></description></item>
    ///     <item><description><see cref="FillRequestInfoMiddleware"/></description></item>
    ///     <item><description><see cref="DistributedContextMiddleware"/></description></item>
    ///     <item><description><see cref="ThrottlingMiddleware"/></description></item>
    ///     <item><description><see cref="LoggingMiddleware"/></description></item>
    ///     <item><description><see cref="DatacenterAwarenessMiddleware"/></description></item>
    ///     <item><description><see cref="UnhandledExceptionMiddleware"/></description></item>
    ///     <item><description><see cref="PingApiMiddleware"/></description></item>
    ///     <item><description><see cref="DiagnosticApiMiddleware"/></description></item>
    /// </list>
    /// </summary>
    public static IVostokWebComponentsBuilder DisableVostokMiddleware<TMiddleware>(this IVostokWebComponentsBuilder configurator) =>
        configurator.Configure(options => options.MiddlewareDisabled[typeof(TMiddleware)] = true);

    /// <summary>
    /// <para>Enables the built-in Vostok middleware of given type <typeparamref name="TMiddleware"></typeparamref>.</para>
    /// <para>Use this method to partially enable vostok middlewares after calling <see cref="DisableAllVostokMiddlewares"/>.</para> // todo
    /// <para>Current list of Vostok middlewares:</para>
    /// <list type="bullet">
    ///     <item><description><see cref="HttpContextTweakMiddleware"/></description></item>
    ///     <item><description><see cref="FillRequestInfoMiddleware"/></description></item>
    ///     <item><description><see cref="DistributedContextMiddleware"/></description></item>
    ///     <item><description><see cref="ThrottlingMiddleware"/></description></item>
    ///     <item><description><see cref="LoggingMiddleware"/></description></item>
    ///     <item><description><see cref="DatacenterAwarenessMiddleware"/></description></item>
    ///     <item><description><see cref="UnhandledExceptionMiddleware"/></description></item>
    ///     <item><description><see cref="PingApiMiddleware"/></description></item>
    ///     <item><description><see cref="DiagnosticApiMiddleware"/></description></item>
    /// </list>
    /// </summary>
    public static IVostokWebComponentsBuilder EnableVostokMiddleware<TMiddleware>(this IVostokWebComponentsBuilder configurator) =>
        configurator.Configure(options => options.MiddlewareDisabled[typeof(TMiddleware)] = false);

    /// <summary>
    /// Injects a singleton middleware of type <typeparamref name="TMiddleware"/> before the earliest Vostok middleware.
    /// </summary>
    public static IVostokWebComponentsBuilder InjectPreVostokMiddleware<TMiddleware>(this IVostokWebComponentsBuilder configurator) =>
        configurator.InjectPreVostokMiddleware<TMiddleware, FillRequestInfoMiddleware>();

    /// <summary>
    /// Injects a singleton middleware of type <typeparamref name="TMiddleware"/> before Vostok middleware of type <typeparamref name="TBefore"/>.
    /// </summary>
    public static IVostokWebComponentsBuilder InjectPreVostokMiddleware<TMiddleware, TBefore>(this IVostokWebComponentsBuilder configurator) =>
        configurator.Configure(options =>
        {
            if (!options.PreVostokMiddlewares.TryGetValue(typeof(TBefore), out var injected))
                options.PreVostokMiddlewares[typeof(TBefore)] = injected = [];

            injected.Add(typeof(TMiddleware));
        });
}