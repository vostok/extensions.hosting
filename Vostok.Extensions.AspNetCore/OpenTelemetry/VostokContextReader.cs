using System.Buffers.Binary;
using System.Text;

using Microsoft.Extensions.Logging;

using Vostok.Tracing.Abstractions;

namespace Vostok.Extensions.AspNetCore.OpenTelemetry;

internal sealed class VostokContextReader(ILogger logger)
{
    private const string DistributedGlobalName = "vostok.tracing.context";

    private readonly TraceContextSerializer _traceContextSerializer = new();

    public TraceContext? Read(string input)
    {
        try
        {
            ReadOnlySpan<byte> remainingBytes = Convert.FromBase64String(input);

            do
            {
                remainingBytes = ReadString(remainingBytes, out var key);

                if (key is not DistributedGlobalName)
                    continue;

                ReadString(remainingBytes, out var value, true);
                if (value is not null)
                    return _traceContextSerializer.Deserialize(value);
                break;
            } while (remainingBytes.Length > 0);
        }
        catch (Exception error)
        {
            logger.LogError(error, "Failed to read property names and values from input string '{Input}'.", input);
        }

        return null;
    }

    private static ReadOnlySpan<byte> ReadString(ReadOnlySpan<byte> bytes,
        out string? value,
        bool skipSizeCheck = false)
    {
        const int sizeOfInt = sizeof(int);

        value = null;

        if (!BinaryPrimitives.TryReadInt32LittleEndian(bytes, out var size) || bytes.Length < sizeOfInt + size)
            return ReadOnlySpan<byte>.Empty;

        bytes = bytes[sizeOfInt..];
        if (!skipSizeCheck && size != DistributedGlobalName.Length)
            return bytes[size..];

        var propertyBytes = bytes[..size];
        value = Encoding.UTF8.GetString(propertyBytes);

        return bytes[size..];
    }
}