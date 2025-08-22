using System.ComponentModel;

using Vostok.Configuration.Primitives;
using Vostok.Extensions.Hosting.Helpers.TypeConverters;

namespace Vostok.Extensions.Hosting.Tests.Helpers.TypeConverters;

[TestFixture]
internal sealed class VostokTypeConvertersTests
{
    [OneTimeSetUp]
    public static void SetUp()
        => VostokTypeConverterHelper.AddVostokTypeConverters();

    [TestCaseSource(typeof(DataSizeTestCaseSources), nameof(DataSizeTestCaseSources.Success))]
    [TestCaseSource(typeof(DecimalTestCaseSources), nameof(DecimalTestCaseSources.Success))]
    [TestCaseSource(typeof(DoubleTestCaseSources), nameof(DoubleTestCaseSources.Success))]
    [TestCaseSource(typeof(FloatTestCaseSources), nameof(FloatTestCaseSources.Success))]
    [TestCaseSource(typeof(DateTimeTestCaseSources), nameof(DateTimeTestCaseSources.Success))]
    [TestCaseSource(typeof(DateTimeOffsetTestCaseSources), nameof(DateTimeOffsetTestCaseSources.Success))]
    public void Should_Convert<T>(string input, object expectedValue, string serializedValueExpected)
    {
        var converter = TypeDescriptor.GetConverter(typeof(T));

        var deserializedValue = converter.ConvertFromInvariantString(input);
        deserializedValue.Should().Be(expectedValue);

        var serializedValue = converter.ConvertToInvariantString(deserializedValue);
        serializedValue.Should().Be(serializedValueExpected);
    }

    [TestCaseSource(typeof(DataSizeTestCaseSources), nameof(DataSizeTestCaseSources.Fail))]
    [TestCaseSource(typeof(DecimalTestCaseSources), nameof(DecimalTestCaseSources.Fail))]
    [TestCaseSource(typeof(DoubleTestCaseSources), nameof(DoubleTestCaseSources.Fail))]
    [TestCaseSource(typeof(FloatTestCaseSources), nameof(FloatTestCaseSources.Fail))]
    [TestCaseSource(typeof(DateTimeTestCaseSources), nameof(DateTimeTestCaseSources.Fail))]
    [TestCaseSource(typeof(DateTimeOffsetTestCaseSources), nameof(DateTimeOffsetTestCaseSources.Fail))]
    public void Should_Not_Convert<T>(string input)
    {
        var converter = TypeDescriptor.GetConverter(typeof(T));

        var convert = () => converter.ConvertFromInvariantString(input);
        convert.Should().Throw<Exception>();
    }

    private sealed class DataSizeTestCaseSources : TestCaseSourcesBase<DataSize>
    {
        public static IEnumerable<TestCaseData> Success() =>
        [
            Get("3 Mb", 3.Megabytes(), "3 MB"),
            Get("8 GB  ", 8.Gigabytes(), "8 GB"),
            Get("     12 Kb     ", 12.Kilobytes(), "12 KB"),
            Get("42", 42.Bytes(), "42 B")
        ];

        public static IEnumerable<TestCaseData> Fail() =>
        [
            Get("3 Kg"),
            Get("Hello World!"),
            Get("true"),
            Get(" "),
            Get("_42__")
        ];
    }

    private sealed class DecimalTestCaseSources : TestCaseSourcesBase<decimal>
    {
        public static IEnumerable<TestCaseData> Success() =>
        [
            Get("0", 0m, "0"),
            Get("123", 123m, "123"),
            Get("-5", -5m, "-5"),
            Get("1.23", 1.23m, "1.23"),
            Get("1,23", 1.23m, "1.23"),
            Get("1e3", 1000m, "1000"),
            Get("1.23E-4", 0.000123m, "0.000123"),
            Get("1.2.3.4.5", 12345m, "12345"),
            Get("1,2,3,4,5", 12345m, "12345"),
            Get("   123,45   ", 123.45m, "123.45"),
            Get("79228162514264337593543950335", decimal.MaxValue, "79228162514264337593543950335")
        ];

        public static IEnumerable<TestCaseData> Fail() =>
        [
            Get("bububu"),
            Get("one"),
            Get(""),
            Get("   ")
        ];
    }

    private sealed class DoubleTestCaseSources : TestCaseSourcesBase<double>
    {
        public static IEnumerable<TestCaseData> Success() =>
        [
            Get("0", 0, "0"),
            Get("1", 1, "1"),
            Get("-5", -5, "-5"),
            Get("1.23", 1.23, "1.23"),
            Get("1,23", 1.23, "1.23"),
            Get("   1,23  ", 1.23, "1.23"),
            Get("0.5", 0.5, "0.5"),
            Get("0,5", 0.5, "0.5"),
            Get("1e3", 1000, "1000"),
            Get("1.23E-4", 0.000123, "0.000123"),
            Get("1.2.3.4.5", 12345, "12345"),
            Get("1,2,3,4,5", 12345, "12345"),
            Get("NaN", double.NaN, "NaN"),
            Get("Infinity", double.PositiveInfinity, "Infinity"),
            Get("-Infinity", double.NegativeInfinity, "-Infinity")
        ];

        public static IEnumerable<TestCaseData> Fail() =>
        [
            Get("bububu"),
            Get("one"),
            Get("NaNabc"),
            Get(""),
            Get("   ")
        ];
    }

    private sealed class FloatTestCaseSources : TestCaseSourcesBase<float>
    {
        public static IEnumerable<TestCaseData> Success() =>
        [
            Get("0", 0f, "0"),
            Get("1", 1f, "1"),
            Get("-5", -5f, "-5"),
            Get("1.23", 1.23f, "1.23"),
            Get("1,23", 1.23f, "1.23"),
            Get("   1,23  ", 1.23f, "1.23"),
            Get("1.2.3.4.5", 12345f, "12345"),
            Get("1,2,3,4,5", 12345f, "12345"),
            Get("0.5", 0.5f, "0.5"),
            Get("0,5", 0.5f, "0.5"),
            Get("1e3", 1000f, "1000"),
            Get("1.23E-4", 0.000123f, "0.000123"),
            Get("NaN", float.NaN, "NaN"),
            Get("Infinity", float.PositiveInfinity, "Infinity"),
            Get("-Infinity", float.NegativeInfinity, "-Infinity")
        ];

        public static IEnumerable<TestCaseData> Fail() =>
        [
            Get("bububu"),
            Get("one"),
            Get("NaNabc"),
            Get(""),
            Get("   ")
        ];
    }

    private sealed class DateTimeTestCaseSources : TestCaseSourcesBase<DateTime>
    {
        public static IEnumerable<TestCaseData> Success() =>
        [
            Get("2025-08-20", new DateTime(2025, 8, 20), "2025-08-20"),
            Get("2025-08-20 14:30:00", new DateTime(2025, 8, 20, 14, 30, 0), "08/20/2025 14:30:00"),
            Get("   2025-08-20 14:30:00   ", new DateTime(2025, 8, 20, 14, 30, 0), "08/20/2025 14:30:00"),
            Get("20050809T181142+0330", new DateTime(2005, 8, 9, 18, 11, 42), "08/09/2005 18:11:42")
        ];

        public static IEnumerable<TestCaseData> Fail() =>
        [
            Get("2025-13-01"),
            Get("2025-02-30"),
            Get("abc"),
            Get(""),
            Get("   ")
        ];
    }

    private sealed class DateTimeOffsetTestCaseSources : TestCaseSourcesBase<DateTimeOffset>
    {
        public static IEnumerable<TestCaseData> Success() =>
        [
            Get("2025-08-20+00:00", new DateTimeOffset(2025, 8, 20, 0, 0, 0, TimeSpan.Zero), "2025-08-20 +00:00"),
            Get("2025-08-20 14:30:00+02:00", new DateTimeOffset(2025, 8, 20, 14, 30, 0, TimeSpan.FromHours(2)), "08/20/2025 14:30:00 +02:00"),
            Get("   2025-08-20 14:30:00+02:00   ", new DateTimeOffset(2025, 8, 20, 14, 30, 0, TimeSpan.FromHours(2)), "08/20/2025 14:30:00 +02:00"),
            Get("20050809T181142+0330", new DateTimeOffset(2005, 8, 9, 18, 11, 42, 0, TimeSpan.FromMinutes(3 * 60 + 30)), "08/09/2005 18:11:42 +03:30"),
        ];

        public static IEnumerable<TestCaseData> Fail() =>
        [
            Get("2025-13-01+00:00"),
            Get("2025-02-30+00:00"),
            Get("abc"),
            Get("123"),
            Get("123"),
            Get(""),
            Get("   ")
        ];
    }

    private abstract class TestCaseSourcesBase<T>
    {
        protected static TestCaseData Get(params object[] args) =>
            new(args) { TypeArgs = [typeof(T)] };
    }

    // todo:
    // TimeSpan
    // IPAddress
    // IPEndPoint
    // Uri
    // Encoding
    // DataRate
}