using Vostok.Extensions.Hosting.Components.Logging.Substitution;

namespace Vostok.Extensions.Hosting.Helpers;

/// <summary>
/// Substitutable log for Vostok configuration.
/// Used in other components configs/factories to get rid of potential circular dependencies.
/// </summary>
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