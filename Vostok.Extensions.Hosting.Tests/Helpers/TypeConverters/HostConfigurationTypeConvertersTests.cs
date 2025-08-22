using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Vostok.Configuration.Primitives;

namespace Vostok.Extensions.Hosting.Tests.Helpers.TypeConverters;

internal sealed class HostConfigurationTypeConvertersTests
{
    [Test]
    public void HostConfiguration_Should_Use_VostokTypeConverters()
    {
        var host = PrepareHost();

        var options = host.Services.GetRequiredService<IOptions<TestOptions>>().Value;

        options.DecimalValue.Should().Be(123.45m);
        options.DoubleValue.Should().Be(1000);
        options.FloatValue.Should().Be(0.000123f);
        options.DataSizeValue.Should().Be(3.Megabytes());
        options.DataRateValue.Should().Be(3.MegabytesPerSecond());
        options.DateTimeValue.Should().Be(new DateTime(2025, 8, 20, 14, 30, 0));
        options.DateTimeOffsetValue.Should().Be(new DateTimeOffset(2018, 3, 14, 15, 9, 26, 535, TimeSpan.FromMinutes(DateTimeOffset.Now.Offset.TotalMinutes)));
        options.TimeSpanValue.Should().Be(TimeSpan.FromDays(28));
        options.EncodingValue.Should().Be(Encoding.UTF8);
        options.UriValue.Should().Be(new Uri("https://my_host/my_path"));

        var ipAddress = IPAddress.Parse("127.0.0.1");
        options.IpAddressValue.Should().Be(ipAddress);
        options.IpEndPointValue.Should().Be(new IPEndPoint(ipAddress, 42));
    }

    private static IHost PrepareHost()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.UseVostok();

        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["DecimalValue"] = "   123,45   ",
            ["DoubleValue"] = "1e3   ",
            ["FloatValue"] = "     1.23E-4",
            ["DataSizeValue"] = "3 MB",
            ["DataRateValue"] = "3 MB/s",
            ["DateTimeValue"] = "2025-08-20 14:30:00",
            ["DateTimeOffsetValue"] = "2018-03-14T15:09:26.535",
            ["TimeSpanValue"] = "28 Days",
            ["EncodingValue"] = "UTF-8",
            ["UriValue"] = "https://my_host/my_path",
            ["IpAddressValue"] = "127.0.0.1",
            ["IpEndPointValue"] = "127.0.0.1:42",
        });

        builder.Services.AddOptions<TestOptions>().Bind(builder.Configuration);

        return builder.Build();
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    private sealed class TestOptions
    {
        public required decimal DecimalValue { get; init; }
        public required double DoubleValue { get; init; }
        public required float FloatValue { get; init; }
        public required DataSize DataSizeValue { get; init; }
        public required DataRate DataRateValue { get; init; }
        public required DateTime DateTimeValue { get; init; }
        public required DateTimeOffset DateTimeOffsetValue { get; init; }
        public required TimeSpan TimeSpanValue { get; init; }
        public required Encoding EncodingValue { get; init; }
        public required Uri UriValue { get; init; }
        public required IPAddress IpAddressValue { get; init; }
        public required IPEndPoint IpEndPointValue { get; init; }
    }
}