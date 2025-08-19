using System.ComponentModel;
using System.Globalization;

using Vostok.Commons.Helpers;

namespace Vostok.Extensions.Hosting.Helpers.TypeConverters;

public class VostokNumericConverter<T> : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string text)
        {
            text = text.Trim();

            // note (d.stukov, 19.08.2025): we must throw here when incorrect value because we inherit from base class
            // and have no other options to parse numeric types
            return NumericTypeParser<T>.Parse(text);
        }

        return base.ConvertFrom(context, culture, value);
    }
}