using System.Collections.Concurrent;

using Microsoft.AspNetCore.Http;

using Vostok.Commons.Collections;
using Vostok.Commons.Helpers.Disposable;
using Vostok.Extensions.AspNetCore.Models;

namespace Vostok.Extensions.AspNetCore.Diagnostics;

internal sealed class RequestTracker : IRequestTracker
{
    private readonly ConcurrentDictionary<RequestTrackerItem, byte> _items =
        new(64, 256, ByReferenceEqualityComparer<RequestTrackerItem>.Instance);

    public IEnumerable<RequestTrackerItem> CurrentItems => _items.Select(pair => pair.Key);

    public IDisposable Track(HttpContext context, IRequestInfo info)
    {
        var item = new RequestTrackerItem(context.Request.Path, info);

        _items[item] = 0;

        return new ActionDisposable(() => _items.TryRemove(item, out _));
    }
}