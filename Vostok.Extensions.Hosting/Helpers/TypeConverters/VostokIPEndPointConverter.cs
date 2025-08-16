using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;

namespace Vostok.Extensions.Hosting.Helpers.TypeConverters;

public class VostokIPEndPointConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, [NotNullWhen(true)] Type? destinationType)
    {
        return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string text)
        {
            text = text.Trim();

            if (TryParse(text, out var timeSpan))
                return timeSpan;
        }

        return base.ConvertFrom(context, culture, value);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(InstanceDescriptor) && value is IPEndPoint)
        {
            return new InstanceDescriptor(
                typeof(IPEndPoint).GetMethod(nameof(IPEndPoint.Parse), [typeof(string)]),
                new object?[] { value.ToString() }
            );
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
    
    // from Vostok.Configuration.Parsers.IPEndPointParser
    private static bool TryParse(string input, out IPEndPoint result)
    {
        result = null;
        IPAddress ipRes;

        var sepPos = input.LastIndexOf(':');
        if (sepPos == -1 || input.EndsWith("]") || !input.Contains(".") && !input.Contains("]:"))
        {
            input = input.Trim('[', ']');
            if (IPAddress.TryParse(input, out ipRes))
            {
                result = new IPEndPoint(ipRes, 0);
                return true;
            }

            return false;
        }

        var ip = input.Substring(0, sepPos).Trim('[', ']');
        var port = input.Substring(sepPos + 1, input.Length - sepPos - 1);
        if (IPAddress.TryParse(ip, out ipRes) && int.TryParse(port, out var portRes))
        {
            result = new IPEndPoint(ipRes, portRes);
            return true;
        }

        return false;
    }
}