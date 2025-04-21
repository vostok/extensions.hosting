using Microsoft.Extensions.Logging;

using OpenTelemetry.Context.Propagation;

using Vostok.Context;
using Vostok.Extensions.AspNetCore.OpenTelemetry;
using Vostok.Tracing;
using Vostok.Tracing.Abstractions;

namespace Vostok.Extensions.AspNetCore.Tests.OpenTelemetry;

internal sealed class VostokContextPropagatorTests
{
    private VostokContextPropagator _vostokContextPropagator = null!;
    private TraceContext _traceContext = null!;

    private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(_ => { });

    [SetUp]
    public void SetUp()
    {
        FlowingContext.Configuration.ClearDistributedGlobals();
        FlowingContext.Configuration.RegisterDistributedGlobal(VostokContextReader.DistributedGlobalName, new TraceContextSerializer());

        _vostokContextPropagator = new VostokContextPropagator(_loggerFactory);
        _traceContext = new TraceContext(Guid.NewGuid(), Guid.NewGuid());
    }

    [Test]
    public void Should_extract_trace_data()
    {
        FlowingContext.Globals.Use(_traceContext);

        var serialized = FlowingContext.SerializeDistributedGlobals()!;

        var result = Extract(serialized);
        CheckResult(result);
    }

    [TestCase("hello")]
    [TestCase("vostok.tracing.contexx")]
    public void Should_extract_trace_data_and_skip_other_fields(string stringKey)
    {
        FlowingContext.Globals.Use(42);
        FlowingContext.Globals.Use(stringKey);
        FlowingContext.Globals.Use(_traceContext);

        FlowingContext.Configuration.RegisterDistributedGlobal("int", new IntSerializer());
        FlowingContext.Configuration.RegisterDistributedGlobal("string", new StringSerializer());

        var serialized = FlowingContext.SerializeDistributedGlobals()!;

        var result = Extract(serialized);
        CheckResult(result);
    }

    private void CheckResult(PropagationContext result)
    {
        result.ActivityContext.TraceId.ToString().Should().Be(_traceContext.TraceId.ToString("N"));
        result.ActivityContext.SpanId.ToString().Should().Be(_traceContext.SpanId.ToString("N")[..16]);
    }

    private PropagationContext Extract(string input)
    {
        return _vostokContextPropagator.Extract(
            default,
            input,
            (carrier, key) =>
            {
                return key == VostokContextPropagator.ContextGlobalsHeader
                    ? new[] {carrier}
                    : Array.Empty<string>();
            });
    }

    internal class IntSerializer : IContextSerializer<int>
    {
        public string Serialize(int value) =>
            value.ToString();

        public int Deserialize(string input) =>
            int.Parse(input);
    }

    internal class StringSerializer : IContextSerializer<string>
    {
        public string Serialize(string value) =>
            value;

        public string Deserialize(string input) =>
            input;
    }
}
