using Vostok.Extensions.Hosting.Components.Logging.Substitution;

namespace Vostok.Extensions.Hosting.Helpers;

internal static class InitializationLogProvider
{
    private static readonly Lazy<SubstitutableLog> Log =
        new(() => new SubstitutableLog(), LazyThreadSafetyMode.ExecutionAndPublication);

    public static SubstitutableLog Get() => Log.Value;
}