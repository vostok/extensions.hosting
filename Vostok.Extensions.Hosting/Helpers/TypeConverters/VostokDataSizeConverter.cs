using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Vostok.Configuration.Primitives;

namespace Vostok.Extensions.Hosting.Helpers.TypeConverters;

internal sealed class VostokDataSizeConverter : TypeConverter
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

            return DataSize.Parse(text);
        }

        return base.ConvertFrom(context, culture, value);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(InstanceDescriptor) && value is DataSize)
        {
            return new InstanceDescriptor(
                typeof(DataSize).GetMethod(nameof(DataSize.Parse), [typeof(string)]),
                new object?[] { value.ToString() }
            );
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}