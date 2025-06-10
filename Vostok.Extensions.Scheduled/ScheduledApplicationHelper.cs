using Microsoft.Extensions.DependencyInjection;

using Vostok.Applications.Scheduled;
using Vostok.Hosting.Abstractions;
using Vostok.Logging.Abstractions;
using Vostok.Tracing.Abstractions;

namespace Vostok.Extensions.Scheduled;

internal static class ScheduledApplicationHelper
{
    public static Task<IScheduledActionsRunner> InitializeAsync(IServiceProvider provider, Action<IScheduledActionsBuilder, IServiceProvider> setup)
    {
        return InitializeAsync(
            provider,
            (builder, p) =>
            {
                setup(builder, p);
                return Task.CompletedTask;
            });
    }

    public static async Task<IScheduledActionsRunner> InitializeAsync(IServiceProvider provider, Func<IScheduledActionsBuilder, IServiceProvider, Task> setup)
    {
        var builder = new ScheduledActionsBuilder(
            provider.GetRequiredService<ILog>(),
            provider.GetRequiredService<ITracer>(),
            provider.GetService<IVostokApplicationDiagnostics>());

        await setup(builder, provider);

        return builder.BuildRunner();
    }
}