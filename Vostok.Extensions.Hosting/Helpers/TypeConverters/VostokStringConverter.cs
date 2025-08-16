using System.ComponentModel;
using System.Globalization;

namespace Vostok.Extensions.Hosting.Helpers.TypeConverters;

public class VostokStringConverter : StringConverter
{
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string stringValue)
            return stringValue;

        return base.ConvertFrom(context, culture, value);
    }
}