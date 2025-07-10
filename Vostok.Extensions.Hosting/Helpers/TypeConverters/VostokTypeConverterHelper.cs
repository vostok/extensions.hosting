using System.ComponentModel;

using Vostok.Configuration.Primitives;

namespace Vostok.Extensions.Hosting.Helpers.TypeConverters;

internal static class VostokTypeConverterHelper
{
    public static void AddVostokTypeConverters()
    {
        // todo: Type descriptors

        // TypeDescriptor.AddAttributes(typeof(string), new TypeConverterAttribute(typeof(VostokStringConverter)));
        // TypeDescriptor.AddAttributes(typeof(decimal), new TypeConverterAttribute(typeof(VostokDecimalConverter)));
        // TypeDescriptor.AddAttributes(typeof(double), new TypeConverterAttribute(typeof(VostokDoubleConverter)));
        // TypeDescriptor.AddAttributes(typeof(float), new TypeConverterAttribute(typeof(VostokFloatConverter)));
        // TypeDescriptor.AddAttributes(typeof(DateTime), new TypeConverterAttribute(typeof(VostokDateTimeConverter)));
        // TypeDescriptor.AddAttributes(typeof(DateTimeOffset), new TypeConverterAttribute(typeof(VostokDateTimeOffsetConverter)));
        TypeDescriptor.AddAttributes(typeof(TimeSpan), new TypeConverterAttribute(typeof(VostokTimeSpanConverter)));
        // TypeDescriptor.AddAttributes(typeof(IPAddress), new TypeConverterAttribute(typeof(VostokIPAddressConverter)));
        // TypeDescriptor.AddAttributes(typeof(IPEndPoint), new TypeConverterAttribute(typeof(VostokIPEndPointConverter)));
        // TypeDescriptor.AddAttributes(typeof(Uri), new TypeConverterAttribute(typeof(VostokUriConverter)));
        // TypeDescriptor.AddAttributes(typeof(Encoding), new TypeConverterAttribute(typeof(VostokEncodingConverter)));
        TypeDescriptor.AddAttributes(typeof(DataSize), new TypeConverterAttribute(typeof(VostokDataSizeConverter)));
        // TypeDescriptor.AddAttributes(typeof(DataRate), new TypeConverterAttribute(typeof(VostokDataRateConverter)));
    }
}