using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;

namespace Vostok.Extensions.Hosting.Helpers.TypeConverters;

public class VostokIPAddressConverter : TypeConverter
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

            return IPAddress.Parse(text);
        }

        return base.ConvertFrom(context, culture, value);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(InstanceDescriptor) && value is IPAddress)
        {
            return new InstanceDescriptor(
                typeof(IPAddress).GetMethod(nameof(IPAddress.Parse), [typeof(string)]),
                new object?[] { value.ToString() }
            );
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}