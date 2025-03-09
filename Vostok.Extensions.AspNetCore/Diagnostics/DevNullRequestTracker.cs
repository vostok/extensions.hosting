using Microsoft.AspNetCore.Http;

using Vostok.Commons.Helpers.Disposable;

namespace Vostok.Extensions.AspNetCore.Diagnostics;

internal sealed class DevNullRequestTracker : IRequestTracker
{
    private readonly IDisposable _result = new ActionDisposable(delegate { });

    public IDisposable Track(HttpContext context, IRequestInfo info) => _result;
}