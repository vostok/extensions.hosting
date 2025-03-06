using JetBrains.Annotations;

using Microsoft.AspNetCore.Http;

using Vostok.Extensions.AspNetCore.Models;

namespace Vostok.Extensions.AspNetCore.Diagnostics;

[PublicAPI]
public interface IRequestTracker
{
    IDisposable Track(HttpContext context, IRequestInfo info);
}