using JetBrains.Annotations;

using Microsoft.AspNetCore.Http;

namespace Vostok.Extensions.AspNetCore.Middlewares.DistributedContext;

/// <summary>
/// Represents configuration of <see cref="DistributedContextMiddleware"/>.
/// </summary>
[PublicAPI]
public sealed class DistributedContextSettings
{
    /// <summary>
    /// Additional actions that will be executed during distributed context restoring.
    /// </summary>
    public List<Action<HttpRequest>> AdditionalActions { get; set; } = [];
}