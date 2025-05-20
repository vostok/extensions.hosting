using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Vostok.Extensions.Hosting.Components.ThreadPool;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class VostokBuilderThreadPoolExtensions
{
    public static IVostokBuilder ConfigureThreadPool(
        this IVostokBuilder builder,
        bool configureThreadPool,
        int? threadPoolTuningMultiplier = null)
    {
        builder.ConfigureServices(services => services.Configure<ThreadPoolOptions>(options =>
        {
            options.ConfigureThreadPool = configureThreadPool;

            if (configureThreadPool && threadPoolTuningMultiplier.HasValue)
                options.ThreadPoolTuningMultiplier = threadPoolTuningMultiplier.Value;
        }));

        return builder;
    }
}