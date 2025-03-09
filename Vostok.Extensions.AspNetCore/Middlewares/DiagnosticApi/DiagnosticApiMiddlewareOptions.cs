using JetBrains.Annotations;

namespace Vostok.Extensions.AspNetCore.Middlewares.DiagnosticApi;

/// <summary>
/// Represents configuration of <see cref="DiagnosticApiMiddleware"/>.
/// </summary>
[PublicAPI]
public sealed class DiagnosticApiMiddlewareOptions
{
    /// <summary>
    /// Url prefix for diagnostic middleware to listen to.
    /// </summary>
    public string PathPrefix { get; set; } = "/_diagnostic";

    /// <summary>
    /// <para>A list of headers whose presence will lead to authorization failure and no response.</para>
    /// <para>Use this list to detect external requests coming via edge servers and proxies.</para>
    /// </summary>
    public List<string> ProhibitedHeaders { get; set; } =
    [
        "X-Real-IP",
        "X-Forwarded-For",
        "Forwarded"
    ];

    /// <summary>
    /// If enabled, the middleware will only respond to requests issued from inside the local host.
    /// </summary>
    public bool AllowOnlyLocalRequests { get; set; }
}