namespace Vostok.Extensions.Hosting.Components.ThreadPool;

internal sealed class ThreadPoolOptions
{
    public bool ConfigureThreadPool { get; set; } = true;
    public int ThreadPoolTuningMultiplier { get; set; } = ThreadPoolConstants.DefaultThreadPoolMultiplier;
}