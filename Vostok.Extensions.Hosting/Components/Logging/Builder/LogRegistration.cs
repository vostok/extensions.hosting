namespace Vostok.Extensions.Hosting.Components.Logging.Builder;

internal sealed class LogRegistration
{
    private readonly string? _key;

    public static readonly LogRegistration All = new();
    public static readonly LogRegistration Unnamed = For("_Unnamed");
    public static LogRegistration For(string key) => new(key);

    private LogRegistration(string? key = null) => _key = key;

    private bool Equals(LogRegistration? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;

        if (_key is null || other._key is null)
            return true;
        return _key == other._key;
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
        _key is null ? 0 : _key.GetHashCode();
}