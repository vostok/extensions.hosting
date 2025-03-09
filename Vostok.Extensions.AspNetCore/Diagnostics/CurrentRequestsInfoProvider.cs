using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Extensions.AspNetCore.Diagnostics;

internal sealed class CurrentRequestsInfoProvider(RequestTracker tracker) : IDiagnosticInfoProvider
{
    public object Query()
    {
        var requests = tracker.CurrentItems
                              .Select(item => new { item.Path, item.Info.ElapsedTime })
                              .OrderByDescending(item => item.ElapsedTime)
                              .ToArray();

        return new
        {
            Count = requests.Length,
            Requests = requests
        };
    }
}