namespace Vostok.Extensions.AspNetCore.Middlewares;

/// <summary>
/// Represents additional configuration of Vostok middlewares.
/// </summary>
internal sealed class VostokMiddlewaresOptions
{
    public bool MiddlewaresAdded { get; set; }

    public readonly Dictionary<Type, bool> MiddlewareDisabled = new();
    public readonly Dictionary<Type, List<Type>> PreVostokMiddlewares = new();

    public bool IsEnabled<TMiddleware>()
    {
        if (!MiddlewaresAdded)
            return false;

        if (MiddlewareDisabled.TryGetValue(typeof(TMiddleware), out var disabled))
            return !disabled;

        return true;
    }
}