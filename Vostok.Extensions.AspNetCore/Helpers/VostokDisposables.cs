using Microsoft.Extensions.Logging;

namespace Vostok.Extensions.AspNetCore.Helpers;

internal sealed class VostokDisposables(ILogger<VostokDisposables> logger) : IDisposable
{
    private readonly List<IDisposable> _disposables = [];

    public void Add(IDisposable disposable)
    {
        lock (_disposables)
        {
            _disposables.Add(disposable);
        }
    }

    public void Dispose()
    {
        lock (_disposables)
        {
            logger.LogInformation("Disposing of Disposables list ({Count} element(s))..", _disposables.Count);
            _disposables.ForEach(disposable => disposable.Dispose());
            _disposables.Clear();
        }
    }
}