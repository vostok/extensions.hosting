using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Vostok.Commons.Helpers;
using Vostok.Configuration.Primitives;

namespace Vostok.Extensions.Hosting.Helpers.TypeConverters;

public class VostokNumericConverter<T> : TypeConverter
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

            if (NumericTypeParser<T>.TryParse(text, out var number))
                return number;
        }

        return base.ConvertFrom(context, culture, value);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(InstanceDescriptor) && value is T)
        {
            return new InstanceDescriptor(
                typeof(T).GetMethod(nameof(NumericTypeParser<T>.TryParse), [typeof(string)]),
                new object?[] { value.ToString() }
            );
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}