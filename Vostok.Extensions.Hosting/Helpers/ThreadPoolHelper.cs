using Vostok.Commons.Threading;

namespace Vostok.Extensions.Hosting.Helpers;

internal static class ThreadPoolHelper
{
    public static void Setup(int multiplier, float? cpuLimit)
    {
        if (cpuLimit.HasValue)
            cpuLimit = Math.Max(cpuLimit.Value, 1);

        ThreadPoolUtility.Setup(multiplier, cpuLimit);
    }
}