namespace Vostok.Extensions.Hosting.Components.Logging.Builder;

internal sealed class LogRegistration
{
    private readonly string? _name;
    public string Name => _name ?? throw new InvalidOperationException($"Cannot get a log name from '{nameof(All)}' registration.");

    public static readonly LogRegistration All = new();
    public static LogRegistration For(string name) => new(name ?? throw new ArgumentNullException(nameof(name)));

    private LogRegistration(string? name = null) => _name = name;

    private bool Equals(LogRegistration? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;

        if (_name is null || other._name is null)
            return true;
        return _name == other._name;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Equals((LogRegistration)obj);
    }

    public override int GetHashCode() =>
        _name is null ? 0 : _name.GetHashCode();
}