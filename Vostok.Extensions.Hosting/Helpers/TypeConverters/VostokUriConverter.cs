using System.ComponentModel;
using System.Globalization;

namespace Vostok.Extensions.Hosting.Helpers.TypeConverters;

public class VostokUriConverter : UriTypeConverter
{
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string uriString)
        {
            // note (d.stukov, 19.08.2025): the only change is the trim here
            uriString = uriString.Trim();

            if (Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out var result))
                return result;

            // note (d.stukov, 19.08.2025): should not throw here (rely on corresponding converter)
        }

        return base.ConvertFrom(context, culture, value);
    }
}