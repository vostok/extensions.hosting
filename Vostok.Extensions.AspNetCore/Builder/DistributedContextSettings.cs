using JetBrains.Annotations;

using Microsoft.AspNetCore.Http;

using Vostok.Extensions.AspNetCore.Middlewares;

namespace Vostok.Extensions.AspNetCore.Builder;

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