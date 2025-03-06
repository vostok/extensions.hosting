using JetBrains.Annotations;

using Microsoft.AspNetCore.Http;

using Vostok.Extensions.AspNetCore.Middlewares;
using Vostok.Throttling;

namespace Vostok.Extensions.AspNetCore.Builder;

/// <summary>
/// Represents configuration of <see cref="ThrottlingMiddleware"/>.
/// </summary>
[PublicAPI]
public class ThrottlingSettings
{
    /// <summary>
    /// <para>HTTP code to respond with when a request fails to pass throttling.</para>
    /// <para>Note that in some circumstances (expired timeout, large request body) client connection may be aborted instead.</para>
    /// </summary>
    public int RejectionResponseCode { get; set; } = 429;

    /// <summary>
    /// If set to <c>true</c>, the middleware will pass <see cref="WellKnownThrottlingProperties.Consumer"/> property to <see cref="IThrottlingProvider"/>.
    /// </summary>
    public bool AddConsumerProperty { get; set; } = true;

    /// <summary>
    /// If set to <c>true</c>, the middleware will pass <see cref="WellKnownThrottlingProperties.Priority"/> property to <see cref="IThrottlingProvider"/>.
    /// </summary>
    public bool AddPriorityProperty { get; set; } = true;

    /// <summary>
    /// If set to <c>true</c>, the middleware will pass <see cref="WellKnownThrottlingProperties.Method"/> property to <see cref="IThrottlingProvider"/>.
    /// </summary>
    public bool AddMethodProperty { get; set; } = true;

    /// <summary>
    /// If set to <c>true</c>, the middleware will pass <see cref="WellKnownThrottlingProperties.Url"/> property to <see cref="IThrottlingProvider"/>.
    /// </summary>
    public bool AddUrlProperty { get; set; }

    /// <summary>
    /// <para>Additional properties to be extracted from <see cref="HttpContext"/> and passed to <see cref="IThrottlingProvider"/>.</para>
    /// <para>Use <see cref="IVostokThrottlingBuilderExtensions.UseCustomPropertyQuota"/> extension to add throttling by a custom property.</para>
    /// </summary>
    public List<Func<HttpContext, (string propertyName, string propertyValue)>> AdditionalProperties { get; } = [];

    /// <summary>
    /// <para>If set to <c>true</c>, disables throttling entirely for web socket requests.</para>
    /// <para>Has precedence over <see cref="Enabled"/>.</para>
    /// </summary>
    public bool DisableForWebSockets { get; set; } = true;

    /// <summary>
    /// An optional delegate to decide whether to do (<c>true</c> return value) or skip (<c>false</c> return value) request throttling.
    /// </summary>
    public Func<HttpContext, bool>? Enabled { get; set; }
}