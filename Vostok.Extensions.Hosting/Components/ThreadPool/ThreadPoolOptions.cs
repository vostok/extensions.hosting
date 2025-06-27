namespace Vostok.Extensions.Hosting.Components.ThreadPool;

internal sealed class ThreadPoolOptions
{
    private const int DefaultThreadPoolMultiplier = 32;

    public bool ConfigureThreadPool { get; set; } = true;
    public int ThreadPoolTuningMultiplier { get; set; } = DefaultThreadPoolMultiplier;
}