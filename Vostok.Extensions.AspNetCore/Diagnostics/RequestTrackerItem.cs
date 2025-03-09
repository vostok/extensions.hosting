namespace Vostok.Extensions.AspNetCore.Diagnostics;

internal sealed record RequestTrackerItem(string Path, IRequestInfo Info);