using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Extensions.Configuration;

using Vostok.Extensions.Hosting.Components.Configuration;


namespace Vostok.Extensions.Hosting.Tests;

[TestFixture]
internal sealed class ConfigurationConverterTests
{
    [Test]
    public void Check_EdgeCases_WithArrayIndices()
    {
        var array = new Check
        {
            ArrayValue = ["10", "20", "30"]
        };

        ShouldBeSameAfterConvertation(array);

        var dict = new Check
        {
            DictValue = new Dictionary<string, string>
            {
                { "404", "not found" },
                { "500", "internal server error" }
            }
        };

        ShouldBeSameAfterConvertation(dict);

        var dict2 = new Check
        {
            DictValue = new Dictionary<string, string>
            {
                { "key1", "value1" },
                { "key2", "value2" }
            }
        };

        ShouldBeSameAfterConvertation(dict2);

        var dict3 = new Check
        {
            DictValue = new Dictionary<string, string>
            {
                { "0", "value1" },
                { "1", "value2" }
            }
        };

        var check = () => ShouldBeSameAfterConvertation(dict3);
        check.Should().Throw<Exception>(because: "will be parsed as array");

        var dict4 = new Check
        {
            DictValue = new Dictionary<string, string>
            {
                { "1", "value2" },
                { "0", "value1" }
            }
        };

        check = () => ShouldBeSameAfterConvertation(dict4);
        check.Should().Throw<Exception>(because: "will be parsed as array too");

        var all = new Check
        {
            ArrayValue = ["10", "Key", "30.08"],
            DictValue = new Dictionary<string, string>
            {
                { "0", "value1" },
                { "1", "value2" },
                { "key", "value" }
            }
        };

        ShouldBeSameAfterConvertation(all);
    }

    private class Check
    {
        public string[]? ArrayValue { get; set; }
        public Dictionary<string, string>? DictValue { get; set; }
    }

    private static void ShouldBeSameAfterConvertation<T>(T originalClass)
    {
        var originalJson = JsonSerializer.Serialize(originalClass, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        originalJson.Length.Should().BeGreaterThan(10); // просто проверка, что нигде не набажили

        var configurationRoot = new ConfigurationBuilder()
            .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(originalJson)))
            .Build();

        var node = ConfigurationConverter.ConvertToSettingsNode(configurationRoot);
        var convertedJson = node.ToString()!;

        var minifiedOriginal = MinifyAndPreserveOrder(originalJson);
        var minifiedConverted = MinifyAndPreserveOrder(convertedJson);
        minifiedOriginal.Should().Be(minifiedConverted);

        var convertedClass = JsonSerializer.Deserialize<T>(convertedJson);
        originalClass.Should().BeEquivalentTo(convertedClass);
    }

    private static string MinifyAndPreserveOrder(string json)
    {
        using var doc = JsonDocument.Parse(json);
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false }))
        {
            doc.WriteTo(writer);
        }
        return Encoding.UTF8.GetString(stream.ToArray());
    }
}