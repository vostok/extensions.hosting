using JetBrains.Annotations;

using Microsoft.AspNetCore.Http;

using Vostok.Clusterclient.Core.Model;

namespace Vostok.Extensions.AspNetCore.Middlewares.FillRequestInfo;

/// <summary>
/// Represents configuration of <see cref="FillRequestInfoMiddleware"/>.
/// </summary>
[PublicAPI]
public sealed class FillRequestInfoMiddlewareOptions
{
    /// <summary>
    /// <para>If set to <c>true</c>, enables current requests diagnostic info provider.</para>
    /// </summary>
    public bool AddCurrentRequestsInfoProvider { get; set; } = true;

    /// <summary>
    /// <para>A delegate that provides a default value for <see cref="Timeout"/>.</para>
    /// <para>Used when the request does not contain any supported headers with timeout value.</para>
    /// <para>Executed after built-in provider and <see cref="AdditionalTimeoutProviders"/>.</para>
    /// </summary>
    public Func<HttpRequest, TimeSpan> DefaultTimeoutProvider { get; set; } = _ => TimeSpan.FromMinutes(1);

    /// <summary>
    /// <para>A delegate that provides a default value for <see cref="IRequestInfo.Priority"/>.</para>
    /// <para>Used when the request does not contain any supported headers with priority value.</para>
    /// <para>Executed after built-in provider and <see cref="AdditionalPriorityProviders"/>.</para>
    /// </summary>
    public Func<HttpRequest, RequestPriority> DefaultPriorityProvider { get; set; } = _ => RequestPriority.Ordinary;

    /// <summary>
    /// <para>A list of optional delegates that obtain <see cref="IRequestInfo.Timeout"/> from <see cref="HttpRequest"/>.</para>
    /// <para>By default, the middleware parses the value of <see cref="HeaderNames.RequestTimeout"/> header.</para>
    /// <para>Provided delegates are not expected to throw exceptions; <c>null</c> should be returned instead.</para>
    /// </summary>
    public List<Func<HttpRequest, TimeSpan?>> AdditionalTimeoutProviders { get; } = [];

    /// <summary>
    /// <para>A list of optional delegates that obtain <see cref="IRequestInfo.Priority"/> from <see cref="HttpRequest"/>.</para>
    /// <para>By default, the middleware parses the value of <see cref="HeaderNames.RequestPriority"/> header.</para>
    /// <para>Provided delegates are not expected to throw exceptions; <c>null</c> should be returned instead.</para>
    /// </summary>
    public List<Func<HttpRequest, RequestPriority?>> AdditionalPriorityProviders { get; } = [];

    /// <summary>
    /// <para>A list of optional delegates that obtain <see cref="IRequestInfo.ClientApplicationIdentity"/> from <see cref="HttpRequest"/>.</para>
    /// <para>By default, the middleware parses the value of<see cref="HeaderNames.ApplicationIdentity"/> header.</para>
    /// <para>Provided delegates are not expected to throw exceptions; <c>null</c> should be returned instead.</para>
    /// </summary>
    public List<Func<HttpRequest, string?>> AdditionalClientIdentityProviders { get; set; } = [];
}