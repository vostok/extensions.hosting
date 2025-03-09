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

    public static LoggingCollectionOptions? ToCaseInsensitive(this LoggingCollectionOptions? settings)
        => settings == null
            ? null
            : new LoggingCollectionOptions(settings.Enabled)
            {
                Whitelist = settings.Whitelist == null ? null : new HashSet<string>(settings.Whitelist, StringComparer.OrdinalIgnoreCase),
                Blacklist = settings.Blacklist == null ? null : new HashSet<string>(settings.Blacklist, StringComparer.OrdinalIgnoreCase)
            };
}