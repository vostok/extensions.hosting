using System.ComponentModel;
using System.Net;
using System.Text;

using Vostok.Configuration.Primitives;

namespace Vostok.Extensions.Hosting.Helpers.TypeConverters;

internal static class VostokTypeConverterHelper
{
    // adopted logic from that class:
    // Vostok.Configuration/Binders/Extensions/SettingsBinderProviderExtensions.cs
    public static void AddVostokTypeConverters()
    {
        // extend existing converters for system types
        TypeDescriptor.AddAttributes(typeof(decimal),
            new TypeConverterAttribute(typeof(VostokNumericConverter<decimal>)));
        TypeDescriptor.AddAttributes(typeof(double),
            new TypeConverterAttribute(typeof(VostokNumericConverter<double>)));
        TypeDescriptor.AddAttributes(typeof(float), new TypeConverterAttribute(typeof(VostokNumericConverter<float>)));
        TypeDescriptor.AddAttributes(typeof(DateTime), new TypeConverterAttribute(typeof(VostokDateTimeConverter)));
        TypeDescriptor.AddAttributes(typeof(DateTimeOffset),
            new TypeConverterAttribute(typeof(VostokDateTimeOffsetConverter)));
        TypeDescriptor.AddAttributes(typeof(TimeSpan), new TypeConverterAttribute(typeof(VostokTimeSpanConverter)));
        TypeDescriptor.AddAttributes(typeof(Uri), new TypeConverterAttribute(typeof(VostokUriConverter)));

        // add converters for system types without ready converters
        TypeDescriptor.AddAttributes(typeof(IPAddress), new TypeConverterAttribute(typeof(VostokIPAddressConverter)));
        TypeDescriptor.AddAttributes(typeof(IPEndPoint), new TypeConverterAttribute(typeof(VostokIPEndPointConverter)));
        TypeDescriptor.AddAttributes(typeof(Encoding), new TypeConverterAttribute(typeof(VostokEncodingConverter)));

        // add converters for vostok types
        TypeDescriptor.AddAttributes(typeof(DataSize), new TypeConverterAttribute(typeof(VostokDataSizeConverter)));
        TypeDescriptor.AddAttributes(typeof(DataRate), new TypeConverterAttribute(typeof(VostokDataRateConverter)));
    }
}