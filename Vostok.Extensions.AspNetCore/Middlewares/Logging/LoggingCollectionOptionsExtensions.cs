using Microsoft.AspNetCore.Http;

namespace Vostok.Extensions.AspNetCore.Middlewares.Logging;

internal static class LoggingCollectionOptionsExtensions
{
    public static bool IsEnabledForRequest(this LoggingCollectionOptions options, HttpRequest request) =>
        options.Enabled(request);

    public static bool IsEnabledForAllKeys(this LoggingCollectionOptions options) =>
        options.Whitelist == null && options.Blacklist == null;

    public static bool IsEnabledForKey(this LoggingCollectionOptions options, string key)
    {
        if (options.Whitelist == null)
            return options.Blacklist == null || !options.Blacklist.Contains(key);

        return options.Whitelist.Contains(key);
    }

    public static LoggingCollectionOptions? ToCaseInsensitive(this LoggingCollectionOptions? options)
        => options == null
            ? null
            : new LoggingCollectionOptions(options.Enabled)
            {
                Whitelist = options.Whitelist == null ? null : new HashSet<string>(options.Whitelist, StringComparer.OrdinalIgnoreCase),
                Blacklist = options.Blacklist == null ? null : new HashSet<string>(options.Blacklist, StringComparer.OrdinalIgnoreCase)
            };
}