using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;

using Vostok.Configuration.Parsers;

namespace Vostok.Extensions.Hosting.Helpers.TypeConverters;

// ReSharper disable once InconsistentNaming
internal class VostokIPEndPointConverter : TypeConverter
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

            // vostok logic
            if (IPEndPointParser.TryParse(text, out var ipEndPoint))
                return ipEndPoint;

            // dotnet logic (should throw when can't parse)
            return IPEndPoint.Parse(text);
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
}