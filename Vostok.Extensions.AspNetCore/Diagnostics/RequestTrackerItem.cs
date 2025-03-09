using Vostok.Extensions.AspNetCore.Models;

namespace Vostok.Extensions.AspNetCore.Diagnostics;

internal sealed class RequestTrackerItem
{
    internal RequestTrackerItem(string path, IRequestInfo info)
    {
        Path = path;
        Info = info;
    }

    public string Path { get; }

    public IRequestInfo Info { get; }
}