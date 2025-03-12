using System.Text;

using Microsoft.Extensions.Logging;

using Vostok.Tracing.Abstractions;

namespace Vostok.Extensions.AspNetCore.OpenTelemetry;

internal sealed class VostokContextReader(ILogger logger)
{
    private const string DistributedGlobalName = "vostok.tracing.context";

    private readonly TraceContextSerializer _traceContextSerializer = new();

    // todo: Replace with safe Span api.
    public unsafe TraceContext? Read(string input)
    {
        try
        {
            var bytes = Convert.FromBase64String(input);

            fixed (byte* begin = bytes)
            {
                var end = begin + bytes.Length;

                var ptr = begin;

                string ReadString(out int size, int? expectedSize = null)
                {
                    size = *(int*)ptr;
                    if (size < 0)
                        throw new Exception("Negative size.");
                    if (expectedSize.HasValue && size != expectedSize)
                        return string.Empty;
                    if (ptr + sizeof(int) + size > end)
                        throw new Exception("Wrong sizes.");
                    return Encoding.UTF8.GetString(ptr + sizeof(int), size);
                }

                while (ptr < end)
                {
                    var key = ReadString(out var size, DistributedGlobalName.Length);
                    if (key == DistributedGlobalName)
                    {
                        ptr += sizeof(int) + size;
                        var value = ReadString(out _);
                        return _traceContextSerializer.Deserialize(value);
                    }

                    ptr += sizeof(int) + size;
                    ReadString(out size, -1);
                    ptr += sizeof(int) + size;
                }
            }
        }
        catch (Exception error)
        {
            logger.LogError(error, "Failed to read property names and values from input string '{Input}'.", input);
        }

        return null;
    }
}