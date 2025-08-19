using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Vostok.Configuration.Primitives;

namespace Vostok.Extensions.Hosting.Helpers.TypeConverters;

public class VostokDataRateConverter : TypeConverter
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

            return DataRate.Parse(text);
        }

        return base.ConvertFrom(context, culture, value);
    }

    // note (d.stukov, 19.08.2025): this logic was adopted from the corresponding System.ComponentModel.TimeSpanConverter method.
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(InstanceDescriptor) && value is DataRate)
        {
            return new InstanceDescriptor(
                typeof(DataRate).GetMethod(nameof(DataRate.Parse), [typeof(string)]),
                new object?[] { value.ToString() }
            );
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}