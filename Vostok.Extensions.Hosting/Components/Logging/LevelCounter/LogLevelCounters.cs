namespace Vostok.Extensions.Hosting.Components.Logging.LevelCounter;

internal sealed class LogLevelCounters
{
    public required int Debug { get; init; }
    public required int Info { get; init; }
    public required int Warn { get; init; }
    public required int Error { get; init; }
    public required int Fatal { get; init; }
}