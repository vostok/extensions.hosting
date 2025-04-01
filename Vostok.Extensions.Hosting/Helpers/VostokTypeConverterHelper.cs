using System.ComponentModel;

using Vostok.Configuration.Primitives;
using Vostok.Extensions.Hosting.TypeConverters;

namespace Vostok.Extensions.Hosting.Helpers;

internal static class VostokTypeConverterHelper
{
    public static void AddVostokAttributes()
    {
        // todo: TypeDescriptor.AddAttributes(typeof(string), new TypeConverterAttribute(typeof(VostokStringConverter)));
        // todo: TypeDescriptor.AddAttributes(typeof(decimal), new TypeConverterAttribute(typeof(VostokDecimalConverter)));
        // todo: TypeDescriptor.AddAttributes(typeof(double), new TypeConverterAttribute(typeof(VostokDoubleConverter)));
        // todo: TypeDescriptor.AddAttributes(typeof(float), new TypeConverterAttribute(typeof(VostokFloatConverter)));
        // todo: TypeDescriptor.AddAttributes(typeof(DateTime), new TypeConverterAttribute(typeof(VostokDateTimeConverter)));
        // todo: TypeDescriptor.AddAttributes(typeof(DateTimeOffset), new TypeConverterAttribute(typeof(VostokDateTimeOffsetConverter)));
        TypeDescriptor.AddAttributes(typeof(TimeSpan), new TypeConverterAttribute(typeof(VostokTimeSpanConverter)));
        // todo: TypeDescriptor.AddAttributes(typeof(IPAddress), new TypeConverterAttribute(typeof(VostokIPAddressConverter)));
        // todo: TypeDescriptor.AddAttributes(typeof(IPEndPoint), new TypeConverterAttribute(typeof(VostokIPEndPointConverter)));
        // todo: TypeDescriptor.AddAttributes(typeof(Uri), new TypeConverterAttribute(typeof(VostokUriConverter)));
        // todo: TypeDescriptor.AddAttributes(typeof(Encoding), new TypeConverterAttribute(typeof(VostokEncodingConverter)));
        TypeDescriptor.AddAttributes(typeof(DataSize), new TypeConverterAttribute(typeof(VostokDataSizeConverter)));
        // todo: TypeDescriptor.AddAttributes(typeof(DataRate), new TypeConverterAttribute(typeof(VostokDataRateConverter)));
    }
}