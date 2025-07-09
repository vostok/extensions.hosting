using Vostok.Extensions.Hosting.Components.Logging.Substitution;

namespace Vostok.Extensions.Hosting.Helpers;

internal static class InitializationLog
{
    public const string Key = "_InitializationLog";

    private static Lazy<SubstitutableLog> log = CreateNew();

    public static SubstitutableLog Get() => log.Value;

    public static void Clear() =>
        log = CreateNew();

    private static Lazy<SubstitutableLog> CreateNew() =>
        new(() => new SubstitutableLog(), LazyThreadSafetyMode.ExecutionAndPublication);
}