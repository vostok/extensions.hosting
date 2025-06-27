using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Components.ThreadPool;

[PublicAPI]
public static class VostokBuilderThreadPoolExtensions
{
    public static IVostokBuilder ConfigureThreadPool(
        this IVostokBuilder vostok,
        bool configureThreadPool,
        int? threadPoolTuningMultiplier = null)
    {
        vostok.Services.Configure<ThreadPoolOptions>(options =>
        {
            options.ConfigureThreadPool = configureThreadPool;

            if (configureThreadPool && threadPoolTuningMultiplier.HasValue)
                options.ThreadPoolTuningMultiplier = threadPoolTuningMultiplier.Value;
        });

        return vostok;
    }
}