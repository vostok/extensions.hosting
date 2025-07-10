using System.Reflection;

using JetBrains.Annotations;

using Vostok.Extensions.Hosting.Components.Diagnostics;

namespace Vostok.Extensions.AspNetCore.Middlewares.PingApi;

/// <summary>
/// Represents configuration of <see cref="PingApiMiddleware"/>.
/// </summary>
[PublicAPI]
public sealed class PingApiMiddlewareOptions
{
    /// <summary>
    /// <para>An optional delegate that returns <c>true</c> if the application is already initialized or <c>false</c> if warmup is still in progress.</para>
    /// </summary>
    public Func<bool>? InitializationCheck { get; set; }

    /// <summary>
    /// <para>An optional delegate that returns <c>true</c> if the application is currently healthy or <c>false</c> if there are some warnings.</para>
    /// <para>By default, uses built-in health-check <see cref="IHealthTracker.CurrentReport"/>.</para>
    /// </summary>
    public Func<bool>? HealthCheck { get; set; }

    /// <summary>
    /// <para>An optional delegate that returns application commit hash.</para>
    /// <para>By default, commit hash is extracted from <see cref="AssemblyTitleAttribute"/> of the entry assembly.</para>
    /// </summary>
    public Func<string>? CommitHashProvider { get; set; }
}