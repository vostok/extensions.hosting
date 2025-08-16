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
            var res = DateTimeOffsetParser.TryParse(stringValue, out var dt);
            return res ? dt.DateTime : default;;
        }
        
        return base.ConvertFrom(context, culture, value);
    }
}