using System.ComponentModel;
using System.Globalization;

using Vostok.Configuration.Parsers;

namespace Vostok.Extensions.Hosting.TypeConverters;

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
}