using JetBrains.Annotations;

using Microsoft.AspNetCore.Http;

namespace Vostok.Extensions.AspNetCore.Diagnostics;

[PublicAPI]
public interface IRequestTracker
{
    IDisposable Track(HttpContext context, IRequestInfo info);
}