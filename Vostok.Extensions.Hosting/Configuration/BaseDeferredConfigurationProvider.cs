using Microsoft.Extensions.Primitives;

using Vostok.Configuration.Abstractions;
using Vostok.Configuration.Microsoft;

using IConfigurationProvider = Microsoft.Extensions.Configuration.IConfigurationProvider;

namespace Vostok.Extensions.Hosting.Configuration;

internal abstract class BaseDeferredConfigurationProvider<TSource> : IConfigurationProvider
    where TSource : IConfigurationSource
{
    private readonly ActivationToken _activationToken = new();
    private VostokConfigurationProvider? _provider;

    public bool TryGet(string key, out string? value)
    {
        value = null;
        return _provider?.TryGet(key, out value) is true;
    }

    public void Set(string key, string? value) => _provider?.Set(key, value);
    public void Load() => _provider?.Load();

    public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string? parentPath) =>
        _provider?.GetChildKeys(earlierKeys, parentPath) ?? earlierKeys;

    public IChangeToken GetReloadToken() => _provider?.GetReloadToken() ?? _activationToken;

    public void Activate(TSource vostokConfigurationSource)
    {
        _provider = new VostokConfigurationProvider(vostokConfigurationSource);
        Load();

        _activationToken.SetActivated();
    }

    private sealed class ActivationToken : CancellationChangeToken
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        public ActivationToken() : this(new CancellationTokenSource()) { }

        private ActivationToken(CancellationTokenSource source) : base(source.Token) =>
            _cancellationTokenSource = source;

        public void SetActivated() => _cancellationTokenSource.Cancel();
    }
}