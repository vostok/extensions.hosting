using System.ComponentModel;
using System.Globalization;

using Vostok.Configuration.Parsers;

namespace Vostok.Extensions.Hosting.Helpers.TypeConverters;

public class VostokDateTimeConverter : DateTimeConverter
{
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string stringValue)
        {
            if (DateTimeParser.TryParse(stringValue, out var dateTime))
                return dateTime;

            // note (d.stukov, 19.08.2025): should not throw here (rely on corresponding converter)
        }

        return base.ConvertFrom(context, culture, value);
    }
}