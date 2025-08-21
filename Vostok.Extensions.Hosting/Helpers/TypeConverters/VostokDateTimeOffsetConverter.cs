using System.ComponentModel;
using System.Globalization;

using Vostok.Configuration.Parsers;

namespace Vostok.Extensions.Hosting.Helpers.TypeConverters;

internal class VostokDateTimeOffsetConverter : DateTimeOffsetConverter
{
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string stringValue)
        {
            if (DateTimeOffsetParser.TryParse(stringValue, out var dateTimeOffset))
                return dateTimeOffset;

            // note (ponomaryovigor): Base class interprets empty string as DateTimeOffset.MinValue. We don't want such behaviour.
            if (string.IsNullOrWhiteSpace(stringValue))
                throw new FormatException("Value is null or whitespace.");

            // note (d.stukov, 19.08.2025): should not throw here (rely on corresponding converter)
        }

        return base.ConvertFrom(context, culture, value);
    }
}