using System.ComponentModel;
using System.Globalization;

using Vostok.Configuration.Parsers;

namespace Vostok.Extensions.Hosting.Helpers.TypeConverters;

internal sealed class VostokTimeSpanConverter : TimeSpanConverter
{
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string stringValue)
        {
            if (TimeSpanParser.TryParse(stringValue, out var timeSpan))
                return timeSpan;
        }

        return base.ConvertFrom(context, culture, value);
    }

    // note (d.stukov, 19.08.2025): ConvertTo is not overridden because external services might rely on standard serialization.
    // For example, "3 min" will be converted to "00:03:00" in ConvertFrom method and must be stored in the system in that form.
}