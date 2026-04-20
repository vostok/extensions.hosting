using Microsoft.Extensions.DependencyInjection;

using Vostok.Extensions.Hosting.Components.Logging.Console;
using Vostok.Extensions.Hosting.Components.Logging.File;
using Vostok.Extensions.Hosting.Components.Logging.Internal;
using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Tests;

[TestFixture]
internal sealed class VostokLogFactoryTests
{
    [Test]
    public void GetAllLogRegistrations_ShouldReturnAllRegisteredLogs()
    {
        var services = new ServiceCollection();
        var builder = new VostokLogBuilder(services);

        builder.AddLog(ConsoleLogFactory.Create, ConsoleLogFactory.LogName);
        builder.AddLog(FileLogFactory.Create, FileLogFactory.LogName);

        var provider = services.BuildServiceProvider();

        VostokLogFactory.GetAllLogRegistrations(provider)
            .Select(x => x.LogName)
            .Should()
            .BeEquivalentTo(ConsoleLogFactory.LogName, FileLogFactory.LogName);
    }

    [Test]
    public void DisableLog_ShouldRemoveLog_FromServiceCollection()
    {
        var services = new ServiceCollection();
        var builder = new VostokLogBuilder(services);

        builder.AddLog(ConsoleLogFactory.Create, ConsoleLogFactory.LogName);
        builder.AddLog(FileLogFactory.Create, FileLogFactory.LogName);

        builder.DisableLog(FileLogFactory.LogName);

        var provider = services.BuildServiceProvider();

        VostokLogFactory.GetAllLogRegistrations(provider)
            .Select(x => x.LogName)
            .Should()
            .BeEquivalentTo(ConsoleLogFactory.LogName);
    }

    [Test]
    public void DisableLog_ShouldRemoveAllRegisteredLogs_FromServiceCollection()
    {
        var services = new ServiceCollection();
        var builder = new VostokLogBuilder(services);

        builder.AddLog(ConsoleLogFactory.Create, ConsoleLogFactory.LogName);
        builder.AddLog(FileLogFactory.Create, FileLogFactory.LogName);

        builder.ClearLogs();

        builder.AddLog(_ => new SilentLog(), "MyLog");

        var provider = services.BuildServiceProvider();

        VostokLogFactory.GetAllLogRegistrations(provider)
            .Select(x => x.LogName)
            .Should()
            .BeEquivalentTo("MyLog");
    }
}