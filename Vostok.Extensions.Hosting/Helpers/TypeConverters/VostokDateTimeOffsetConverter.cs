using System.ComponentModel;
using System.Globalization;

using Vostok.Configuration.Parsers;

namespace Vostok.Extensions.Hosting.Helpers.TypeConverters;

public class VostokDateTimeOffsetConverter : DateTimeOffsetConverter
{
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string stringValue)
        {
            if (DateTimeOffsetParser.TryParse(stringValue, out var dateTimeOffset))
                return dateTimeOffset;
        }

        return base.ConvertFrom(context, culture, value);
    }
}