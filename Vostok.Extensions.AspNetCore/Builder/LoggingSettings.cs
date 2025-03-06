using JetBrains.Annotations;

using Vostok.Extensions.AspNetCore.Middlewares;

namespace Vostok.Extensions.AspNetCore.Builder;

/// <summary>
/// Represents configuration of <see cref="LoggingMiddleware"/>.
/// </summary>
[PublicAPI]
public sealed class LoggingSettings
{
    private LoggingCollectionSettings _logQueryString = false;
    private LoggingCollectionSettings _logRequestHeaders = false;
    private LoggingCollectionSettings _logResponseHeaders = false;

    /// <summary>
    /// Determines whether to log incoming requests.
    /// </summary>
    public bool LogRequests { get; set; } = true;

    /// <summary>
    /// Determines whether to log outbound responses.
    /// </summary>
    public bool LogResponses { get; set; } = true;

    /// <summary>
    /// Determines whether to log full response completion time.
    /// </summary>
    public bool LogResponseCompletion { get; set; }

    /// <summary>
    /// Determines whether to response codes of outbound responses for log.
    /// </summary>
    public int[]? LogResponseStatusCodes { get; set; } = [];

    /// <summary>
    /// <para>Request query parameters logging options.</para>
    /// <para>By default, query parameters are not logged at all.</para>
    /// </summary>
    public LoggingCollectionSettings LogQueryString
    {
        get => _logQueryString;
        set => _logQueryString = value.ToCaseInsensitive() ?? throw new ArgumentNullException(nameof(LogQueryString));
    }

    /// <summary>
    /// <para>Request headers logging options.</para>
    /// <para>By default, request headers are not logged at all.</para>
    /// </summary>
    public LoggingCollectionSettings LogRequestHeaders
    {
        get => _logRequestHeaders;
        set => _logRequestHeaders = value.ToCaseInsensitive() ?? throw new ArgumentNullException(nameof(LogRequestHeaders));
    }

    /// <summary>
    /// <para>Response headers logging options.</para>
    /// <para>By default, response headers are not logged at all.</para>
    /// </summary>
    public LoggingCollectionSettings LogResponseHeaders
    {
        get => _logResponseHeaders;
        set => _logResponseHeaders = value.ToCaseInsensitive() ?? throw new ArgumentNullException(nameof(LogResponseHeaders));
    }
}