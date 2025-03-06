namespace Vostok.Extensions.AspNetCore.Builder;

internal sealed class VostokKestrelBuilder
{
    public const int MaxResponseBufferSize = 64 * 1024;

    private const int MaxRequestBufferSize = 128 * 1024;
}