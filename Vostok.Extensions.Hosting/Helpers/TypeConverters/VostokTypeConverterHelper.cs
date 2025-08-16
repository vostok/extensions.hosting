using System.ComponentModel;
using System.Net;
using System.Text;

using Vostok.Configuration.Primitives;

namespace Vostok.Extensions.Hosting.Helpers.TypeConverters;

internal static class VostokTypeConverterHelper
{
    public static void AddVostokTypeConverters()
    {
        // todo: Type descriptors

        TypeDescriptor.AddAttributes(typeof(string), new TypeConverterAttribute(typeof(VostokStringConverter)));
        TypeDescriptor.AddAttributes(typeof(bool), new TypeConverterAttribute(typeof(BooleanConverter)));
        TypeDescriptor.AddAttributes(typeof(char), new TypeConverterAttribute(typeof(CharConverter)));
        TypeDescriptor.AddAttributes(typeof(decimal),
            new TypeConverterAttribute(typeof(VostokNumericConverter<decimal>)));
        TypeDescriptor.AddAttributes(typeof(double),
            new TypeConverterAttribute(typeof(VostokNumericConverter<double>)));
        TypeDescriptor.AddAttributes(typeof(float), new TypeConverterAttribute(typeof(VostokNumericConverter<float>)));

        TypeDescriptor.AddAttributes(typeof(DateTime), new TypeConverterAttribute(typeof(VostokDateTimeConverter)));
        TypeDescriptor.AddAttributes(typeof(DateTimeOffset),
            new TypeConverterAttribute(typeof(VostokDateTimeOffsetConverter)));
        TypeDescriptor.AddAttributes(typeof(TimeSpan), new TypeConverterAttribute(typeof(VostokTimeSpanConverter)));

        TypeDescriptor.AddAttributes(typeof(IPAddress), new TypeConverterAttribute(typeof(VostokIPAddressConverter)));
        TypeDescriptor.AddAttributes(typeof(IPEndPoint), new TypeConverterAttribute(typeof(VostokIPEndPointConverter)));
        TypeDescriptor.AddAttributes(typeof(Uri), new TypeConverterAttribute(typeof(VostokUriConverter)));

        TypeDescriptor.AddAttributes(typeof(Encoding), new TypeConverterAttribute(typeof(VostokEncodingConverter)));
        TypeDescriptor.AddAttributes(typeof(DataSize), new TypeConverterAttribute(typeof(VostokDataSizeConverter)));
        TypeDescriptor.AddAttributes(typeof(DataRate), new TypeConverterAttribute(typeof(VostokDataRateConverter)));
    }
}