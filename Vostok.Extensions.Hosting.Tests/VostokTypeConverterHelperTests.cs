using FluentAssertions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NUnit.Framework;

using Vostok.Extensions.Hosting.Components.ApplicationIdentity;
using Vostok.Extensions.Hosting.Components.ClusterConfigClient;
using Vostok.Extensions.Hosting.Components.Hercules;
using Vostok.Extensions.Hosting.Components.Logging;
using Vostok.Extensions.Hosting.Components.ServiceDiscovery;

namespace Vostok.Extensions.Hosting.Tests;

[TestFixture]
internal sealed class VostokTypeConverterHelperTests
{
    [Test]
    public void Should_ParseString()
    {
        ShouldBeParsed<string>("1.23", "1.23");
        ShouldBeParsed<string>("1,23", "1,23");
        ShouldBeParsed<string>("ABAij123", "ABAij123");
        ShouldBeParsed<string>("{}", "{}");
        ShouldBeParsed<string>("", "");
        ShouldBeParsed<string>("      ", "      ");
        ShouldBeParsed<string>("B5F6541D-C7A9-4308-AFCB-F44F14D98FD7", "B5F6541D-C7A9-4308-AFCB-F44F14D98FD7");
    }

    [Test]
    public void Should_ParseBoolean()
    {
        ShouldBeParsed<bool>("true", true);
        ShouldBeParsed<bool>("false", false);
        ShouldBeParsed<bool>("True", true);
        ShouldBeParsed<bool>("FALSE", false);
        ShouldBeParsed<bool>("    truE    ", true);

        ShouldNotBeParsed<bool>("0");
        ShouldNotBeParsed<bool>("1");
        ShouldNotBeParsed<bool>("123");
        ShouldNotBeParsed<bool>("bububu");
        ShouldNotBeParsed<bool>("yes");
        ShouldNotBeParsed<bool>("no");
        ShouldNotBeParsed<bool>("on");
        ShouldNotBeParsed<bool>("off");
        ShouldNotBeParsed<bool>("");
        ShouldNotBeParsed<bool>("     ");
    }

    [Test]
    public void Should_ParseChar()
    {
        ShouldBeParsed<char>("a", 'a');
        ShouldBeParsed<char>("Z", 'Z');
        ShouldBeParsed<char>("9", '9');
        ShouldBeParsed<char>("!", '!');
        ShouldBeParsed<char>("   x   ", 'x');

        ShouldNotBeParsed<char>("");
        ShouldNotBeParsed<char>("     ");
        ShouldNotBeParsed<char>("ab");
        ShouldNotBeParsed<char>("123");
        ShouldNotBeParsed<char>("🏧");
    }

    [Test]
    public void Should_ParseDecimal()
    {
        ShouldBeParsed<decimal>("0", 0m);
        ShouldBeParsed<decimal>("123", 123m);
        ShouldBeParsed<decimal>("-5", -5m);
        ShouldBeParsed<decimal>("1.23", 1.23m);
        ShouldBeParsed<decimal>("1,23", 1.23m);
        ShouldBeParsed<decimal>("   123,45   ", 123.45m);
        ShouldBeParsed<decimal>("79228162514264337593543950335", decimal.MaxValue);

        ShouldNotBeParsed<decimal>("bububu");
        ShouldNotBeParsed<decimal>("one");
        ShouldNotBeParsed<decimal>("1e3");
        ShouldNotBeParsed<decimal>("1.23E-4");
        ShouldNotBeParsed<decimal>("1.2.3");
        ShouldNotBeParsed<decimal>("1,2,3");
        ShouldNotBeParsed<decimal>("");
        ShouldNotBeParsed<decimal>("   ");
    }

    [Test]
    public void Should_ParseDouble()
    {
        ShouldBeParsed<double>("0", 0);
        ShouldBeParsed<double>("1", 1);
        ShouldBeParsed<double>("-5", -5);
        ShouldBeParsed<double>("1.23", 1.23);
        ShouldBeParsed<double>("1,23", 1.23);
        ShouldBeParsed<double>("   1,23  ", 1.23);
        ShouldBeParsed<double>("0.5", 0.5);
        ShouldBeParsed<double>("0,5", 0.5);
        ShouldBeParsed<double>("1e3", 1000);
        ShouldBeParsed<double>("1.23E-4", 0.000123);
        ShouldBeParsed<double>("1.2.3.4.5", 12345);
        ShouldBeParsed<double>("1,2,3,4,5", 12345);
        ShouldBeParsed<float>("NaN", float.NaN);
        ShouldBeParsed<float>("Infinity", float.PositiveInfinity);
        ShouldBeParsed<float>("-Infinity", float.NegativeInfinity);

        ShouldNotBeParsed<double>("bububu");
        ShouldNotBeParsed<double>("one");
        ShouldNotBeParsed<double>("NaNabc");
        ShouldNotBeParsed<double>("");
        ShouldNotBeParsed<double>("     ");
    }

    [Test]
    public void Should_ParseFloat()
    {
        ShouldBeParsed<float>("0", 0f);
        ShouldBeParsed<float>("123", 123f);
        ShouldBeParsed<float>("-5", -5f);
        ShouldBeParsed<float>("1.23", 1.23f);
        ShouldBeParsed<float>("1,23", 1.23f);
        ShouldBeParsed<float>("   123,45   ", 123.45f);
        ShouldBeParsed<float>("1e3", 1000f);
        ShouldBeParsed<float>("1.23E-4", 0.000123f);
        ShouldBeParsed<float>("NaN", float.NaN);
        ShouldBeParsed<float>("Infinity", float.PositiveInfinity);
        ShouldBeParsed<float>("-Infinity", float.NegativeInfinity);

        ShouldNotBeParsed<float>("bububu");
        ShouldNotBeParsed<float>("one");
        ShouldNotBeParsed<float>("1.2.3");
        ShouldNotBeParsed<float>("1,2,3");
        ShouldNotBeParsed<float>("");
        ShouldNotBeParsed<float>("   ");
    }

    [Test]
    public void Should_ParseSByte()
    {
        ShouldBeParsed<sbyte>("0", 0);
        ShouldBeParsed<sbyte>("127", sbyte.MaxValue);
        ShouldBeParsed<sbyte>("-128", sbyte.MinValue);
        ShouldBeParsed<sbyte>("42", 42);
        ShouldBeParsed<sbyte>("-42", -42);
        ShouldBeParsed<sbyte>("   10   ", 10);

        ShouldNotBeParsed<sbyte>("128");
        ShouldNotBeParsed<sbyte>("-129");
        ShouldNotBeParsed<sbyte>("1.0");
        ShouldNotBeParsed<sbyte>("abc");
        ShouldNotBeParsed<sbyte>("");
        ShouldNotBeParsed<sbyte>("   ");
        ShouldNotBeParsed<sbyte>("42a");
    }

    [Test]
    public void Should_ParseShort()
    {
        ShouldBeParsed<short>("0", 0);
        ShouldBeParsed<short>("32767", short.MaxValue);
        ShouldBeParsed<short>("-32768", short.MinValue);
        ShouldBeParsed<short>("12345", 12345);
        ShouldBeParsed<short>("-12345", -12345);
        ShouldBeParsed<short>("   42   ", 42);

        ShouldNotBeParsed<short>("32768");
        ShouldNotBeParsed<short>("-32769");
        ShouldNotBeParsed<short>("1.0");
        ShouldNotBeParsed<short>("abc");
        ShouldNotBeParsed<short>("");
        ShouldNotBeParsed<short>("   ");
        ShouldNotBeParsed<short>("123abc");
    }

    [Test]
    public void Should_ParseInt()
    {
        ShouldBeParsed<int>("0", 0);
        ShouldBeParsed<int>("2147483647", int.MaxValue);
        ShouldBeParsed<int>("-2147483648", int.MinValue);
        ShouldBeParsed<int>("123456789", 123456789);
        ShouldBeParsed<int>("-987654321", -987654321);
        ShouldBeParsed<int>("   42   ", 42);

        ShouldNotBeParsed<int>("2147483648");
        ShouldNotBeParsed<int>("-2147483649");
        ShouldNotBeParsed<int>("1.0");
        ShouldNotBeParsed<int>("abc");
        ShouldNotBeParsed<int>("");
        ShouldNotBeParsed<int>("   ");
        ShouldNotBeParsed<int>("123abc");
    }

    [Test]
    public void Should_ParseLong()
    {
        ShouldBeParsed<long>("0", 0L);
        ShouldBeParsed<long>("9223372036854775807", long.MaxValue);
        ShouldBeParsed<long>("-9223372036854775808", long.MinValue);
        ShouldBeParsed<long>("123456789012345", 123456789012345L);
        ShouldBeParsed<long>("-98765432109876", -98765432109876L);
        ShouldBeParsed<long>("   42   ", 42L);

        ShouldNotBeParsed<long>("9223372036854775808");
        ShouldNotBeParsed<long>("-9223372036854775809");
        ShouldNotBeParsed<long>("1.0");
        ShouldNotBeParsed<long>("abc");
        ShouldNotBeParsed<long>("");
        ShouldNotBeParsed<long>("   ");
        ShouldNotBeParsed<long>("123abc");
    }

    [Test]
    public void Should_ParseByte()
    {
        ShouldBeParsed<byte>("0", byte.MinValue);
        ShouldBeParsed<byte>("255", byte.MaxValue);
        ShouldBeParsed<byte>("123", 123);
        ShouldBeParsed<byte>("   42   ", 42);

        ShouldNotBeParsed<byte>("256");
        ShouldNotBeParsed<byte>("-1");
        ShouldNotBeParsed<byte>("1.0");
        ShouldNotBeParsed<byte>("abc");
        ShouldNotBeParsed<byte>("");
        ShouldNotBeParsed<byte>("   ");
        ShouldNotBeParsed<byte>("123abc");
    }

    [Test]
    public void Should_ParseUShort()
    {
        ShouldBeParsed<ushort>("0", ushort.MinValue);
        ShouldBeParsed<ushort>("65535", ushort.MaxValue);
        ShouldBeParsed<ushort>("12345", 12345);
        ShouldBeParsed<ushort>("   42   ", 42);

        ShouldNotBeParsed<ushort>("65536");
        ShouldNotBeParsed<ushort>("-1");
        ShouldNotBeParsed<ushort>("1.0");
        ShouldNotBeParsed<ushort>("abc");
        ShouldNotBeParsed<ushort>("");
        ShouldNotBeParsed<ushort>("   ");
        ShouldNotBeParsed<ushort>("123abc");
    }

    [Test]
    public void Should_ParseUInt()
    {
        ShouldBeParsed<uint>("0", uint.MinValue);
        ShouldBeParsed<uint>("4294967295", uint.MaxValue);
        ShouldBeParsed<uint>("1234567890", 1234567890);
        ShouldBeParsed<uint>("   42   ", 42);

        ShouldNotBeParsed<uint>("4294967296");
        ShouldNotBeParsed<uint>("-1");
        ShouldNotBeParsed<uint>("1.0");
        ShouldNotBeParsed<uint>("abc");
        ShouldNotBeParsed<uint>(""); 
        ShouldNotBeParsed<uint>("   ");
        ShouldNotBeParsed<uint>("123abc");
    }

    [Test]
    public void Should_ParseULong()
    {
        ShouldBeParsed<ulong>("0", ulong.MinValue);
        ShouldBeParsed<ulong>("18446744073709551615", ulong.MaxValue);
        ShouldBeParsed<ulong>("12345678901234567890", 12345678901234567890UL);
        ShouldBeParsed<ulong>("   42   ", 42UL);

        ShouldNotBeParsed<ulong>("18446744073709551616");
        ShouldNotBeParsed<ulong>("-1");
        ShouldNotBeParsed<ulong>("1.0");
        ShouldNotBeParsed<ulong>("abc");
        ShouldNotBeParsed<ulong>(""); 
        ShouldNotBeParsed<ulong>("   ");
        ShouldNotBeParsed<ulong>("123abc");
    }

    [Test]
    public void Should_ParseDateTime()
    {
        ShouldBeParsed<DateTime>("2025-08-20", new DateTime(2025, 8, 20));
        ShouldBeParsed<DateTime>("2025-08-20 14:30:00", new DateTime(2025, 8, 20, 14, 30, 0));
        ShouldBeParsed<DateTime>("   2025-08-20 14:30:00   ", new DateTime(2025, 8, 20, 14, 30, 0));
        ShouldBeParsed<DateTime>("20050809T181142+0330", new DateTime(2005, 8, 9, 18, 11, 42));

        ShouldNotBeParsed<DateTime>("2025-13-01");
        ShouldNotBeParsed<DateTime>("2025-02-30");
        ShouldNotBeParsed<DateTime>("abc");
        ShouldNotBeParsed<DateTime>("");
        ShouldNotBeParsed<DateTime>("   ");
    }

    [Test]
    public void Should_ParseDateTimeOffset()
    {
        ShouldBeParsed<DateTimeOffset>("2025-08-20+00:00", new DateTimeOffset(2025, 8, 20, 0, 0, 0, TimeSpan.Zero));
        ShouldBeParsed<DateTimeOffset>("2025-08-20 14:30:00+02:00", new DateTimeOffset(2025, 8, 20, 14, 30, 0, TimeSpan.FromHours(2)));
        ShouldBeParsed<DateTimeOffset>("   2025-08-20 14:30:00+02:00   ", new DateTimeOffset(2025, 8, 20, 14, 30, 0, TimeSpan.FromHours(2)));
        ShouldBeParsed<DateTimeOffset>("2018-03-14 15:09:26.535", new DateTimeOffset(2018, 3, 14, 15, 9, 26, 535, TimeSpan.FromMinutes(DateTimeOffset.Now.Offset.TotalMinutes)));
        ShouldBeParsed<DateTimeOffset>("2018-03-14T15:09:26.535", new DateTimeOffset(2018, 3, 14, 15, 9, 26, 535, TimeSpan.FromMinutes(DateTimeOffset.Now.Offset.TotalMinutes)));
        ShouldBeParsed<DateTimeOffset>("20050809T181142+0330", new DateTimeOffset(2005, 8, 9, 18, 11, 42, 0, TimeSpan.FromMinutes(3*60+30)));
        // todo: should parse that
        // ShouldBeParsed<DateTimeOffset>("20050809T181142", new DateTimeOffset(2005, 8, 9, 18, 11, 42, 0, TimeSpan.FromMinutes(DateTimeOffset.Now.Offset.TotalMinutes)));
        // ShouldBeParsed<DateTimeOffset>("20050809", new DateTimeOffset(2005, 8, 9, 0, 0, 0, 0, TimeSpan.FromMinutes(DateTimeOffset.Now.Offset.TotalMinutes)));
        // ShouldBeParsed<DateTimeOffset>("2005/08/09", new DateTimeOffset(2005, 8, 9, 0, 0, 0, 0, TimeSpan.FromMinutes(DateTimeOffset.Now.Offset.TotalMinutes)));
        // ShouldBeParsed<DateTimeOffset>("2005.08.09", new DateTimeOffset(2005, 8, 9, 0, 0, 0, 0, DateTimeOffset.Now.Offset));
        ShouldBeParsed<DateTimeOffset>("11:22:33", new DateTimeOffset(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 22, 33, 0, TimeSpan.FromMinutes(DateTimeOffset.Now.Offset.TotalMinutes)));
        ShouldBeParsed<DateTimeOffset>("11:22:33.044", new DateTimeOffset(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 22, 33, 44, TimeSpan.FromMinutes(DateTimeOffset.Now.Offset.TotalMinutes)));
        ShouldBeParsed<DateTimeOffset>("112233", new DateTimeOffset(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 22, 33, 0, TimeSpan.FromMinutes(DateTimeOffset.Now.Offset.TotalMinutes)));

        ShouldNotBeParsed<DateTimeOffset>("2025-13-01+00:00");
        ShouldNotBeParsed<DateTimeOffset>("2025-02-30+00:00");
        ShouldNotBeParsed<DateTimeOffset>("abc");
        ShouldNotBeParsed<DateTimeOffset>("123");
        ShouldNotBeParsed<DateTimeOffset>("");
        ShouldNotBeParsed<DateTimeOffset>("   ");
    }

    // todo:
    // TimeSpan
    // IPAddress
    // IPEndPoint
    // Guid
    // Uri
    // Encoding
    // DataSize
    // DataRate

    #region Infrastructure

    private void ShouldBeParsed<T>(string value, T? expected = null)
        where T : class
    {
        var host = SetupHost<T>(value, expected);

        var check = () => host.RunAsync().GetAwaiter().GetResult();
        check.Should().NotThrow();
    }

    private void ShouldBeParsed<T>(string value, T? expected = null)
        where T : struct
    {
        var host = SetupHost<T>(value, expected);

        var check = () => host.RunAsync().GetAwaiter().GetResult();
        check.Should().NotThrow();
    }

    private void ShouldNotBeParsed<T>(string value, RequireClass<T> _ = default)
        where T : class
    {
        var host = SetupHost<T>(value, null);

        var check = () => host.RunAsync().GetAwaiter().GetResult();
        check.Should().Throw<Exception>();
    }

    private void ShouldNotBeParsed<T>(string value, RequireStruct<T> _ = default)
        where T : struct
    {
        var host = SetupHost<T>(value, null);

        var check = () => host.RunAsync().GetAwaiter().GetResult();
        check.Should().Throw<Exception>();
    }

    private static IHost SetupHost<T>(string value, T? expected)
        where T : struct
    {
        var builder = SetupCommonHost<T>(value);

        if (expected.HasValue)
            builder.Services.AddSingleton<ExpectedValueProvider<T>>(_ => () => expected.Value);

        var host = builder.Build();
        return host;
    }

    private static IHost SetupHost<T>(string value, T? expected)
        where T : notnull
    {
        var builder = SetupCommonHost<T>(value);

        if (expected != null)
            builder.Services.AddSingleton<ExpectedValueProvider<T>>(_ => () => expected);

        var host = builder.Build();
        return host;
    }

    private static HostApplicationBuilder SetupCommonHost<T>(string value)
        where T : notnull
    {
        var builder = Host.CreateApplicationBuilder();

        var vostok = builder.UseVostok();
        vostok.WithLogging(x => x.WithConsole());
        vostok.WithApplicationIdentity(x =>
        {
            x.Project = "Project";
            x.Subproject = "Subproject";
            x.Application = "Application";
            x.Environment = "Environment";
            x.Instance = "Instance";
        });
        vostok.DisableClusterConfig()
            .DisableHerculesSink()
            .DisableServiceBeacon()
            .DisableZooKeeper();

        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
        {
            { "Value", value }
        }!);
        builder.Services.Configure<Settings<T>>(builder.Configuration);
        builder.Services.AddHostedService<CheckSettingsHostedService<T>>();

        return builder;
    }

    private class CheckSettingsHostedService<T>(
        IHostApplicationLifetime applicationLifetime,
        IOptionsMonitor<Settings<T>> settingsMonitor,
        ILogger<CheckSettingsHostedService<T>> logger,
        ExpectedValueProvider<T> expectedValueProvider)
        : IHostedService
        where T : notnull
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var oldValue = settingsMonitor.CurrentValue.Value;
            settingsMonitor.CurrentValue.Value = settingsMonitor.CurrentValue.Value;
            var newValue = settingsMonitor.CurrentValue.Value;

            logger.LogInformation($"Old value: {oldValue}, New value: {newValue}");

            var expected = expectedValueProvider();
            if (!expected.Equals(newValue))
                throw new Exception($"Expected: {expected}, Actual: {newValue}");

            applicationLifetime.StopApplication();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }

    private class Settings<T>
        where T : notnull
    {
        public required T Value { get; set; }
    }

    // заглушки, чтобы вызывать одинаковые методы для классов и структур
    private struct RequireClass<T>
        where T : class?
    {
    }

    private struct RequireStruct<T>
        where T : struct
    {
    }

    private delegate T ExpectedValueProvider<out T>();

    #endregion
}
