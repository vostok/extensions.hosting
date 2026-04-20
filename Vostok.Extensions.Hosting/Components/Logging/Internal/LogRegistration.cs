using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Logging.Internal;

internal sealed class LogRegistration(string logName, Func<IServiceProvider, ILog> implementationFactory)
{
    private static int unnamedLogsCounter;

    public ILog Create(IServiceProvider provider)
    {
        var log = implementationFactory(provider);

        var options = provider.GetRequiredService<IOptionsMonitor<VostokLogMiscellaneousOptions>>().Get(logName);

        foreach (var customization in options.Customizations)
            log = customization(log, provider);

        if (options.MinLevelProvider is not null)
            log = log.WithMinimumLevel(options.MinLevelProvider);

        return log;
    }

    public static string GetNextName() => $"Log-{Interlocked.Increment(ref unnamedLogsCounter)}";

    public readonly struct Key : IEquatable<Key>
    {
        private readonly string? _name;

        public static Key For(string name) => new(name);

        private Key(string? name = null) => _name = name;

        public bool Equals(Key other)
        {
            if (_name is null || other._name is null)
                return true;
            return _name == other._name;
        }

        public override bool Equals(object? obj) =>
            obj is Key other && Equals(other);

        public override int GetHashCode() =>
            _name is null ? 0 : _name.GetHashCode();
    }
}