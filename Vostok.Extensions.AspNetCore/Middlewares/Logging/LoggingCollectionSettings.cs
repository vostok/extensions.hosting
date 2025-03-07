using JetBrains.Annotations;

using Microsoft.AspNetCore.Http;

namespace Vostok.Extensions.AspNetCore.Middlewares.Logging;

/// <summary>
/// Configuration of the <see cref="LoggingSettings.LogQueryString"/>, <see cref="LoggingSettings.LogRequestHeaders"/> and <see cref="LoggingSettings.LogResponseHeaders"/>.
/// </summary>
[PublicAPI]
public sealed class LoggingCollectionSettings
{
    public LoggingCollectionSettings(Func<HttpRequest, bool> enabled) =>
        Enabled = enabled ?? throw new ArgumentNullException(nameof(enabled));

    public LoggingCollectionSettings(bool enabled)
        : this(_ => enabled)
    {
    }

    /// <summary>
    /// A delegate that decides whether or not to log parameters of given <see cref="HttpRequest"/> or <see cref="HttpResponse"/>.
    /// </summary>
    public Func<HttpRequest, bool> Enabled { get; }

    /// <summary>
    /// <para>Case-insensitive whitelist of parameter keys to be logged.</para>
    /// <para><c>null</c> value allows all keys.</para>
    /// <para>Takes precedence over <see cref="Blacklist"/>.</para>
    /// </summary>
    public IReadOnlyCollection<string>? Whitelist { get; set; }

    /// <summary>
    /// <para>Case-insensitive blacklist of parameter keys to be logged.</para>
    /// <para><c>null</c> value allows all keys.</para>
    /// </summary>
    public IReadOnlyCollection<string>? Blacklist { get; set; }

    public static implicit operator LoggingCollectionSettings(bool enabled) => new(enabled);
}