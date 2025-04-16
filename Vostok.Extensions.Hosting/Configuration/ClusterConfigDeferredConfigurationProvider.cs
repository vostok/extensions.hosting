using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

using Vostok.Configuration.Microsoft;
using Vostok.Configuration.Sources.ClusterConfig;

namespace Vostok.Extensions.Hosting.Configuration;

internal sealed class ClusterConfigDeferredConfigurationProvider : IConfigurationProvider
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

    public void Activate(ClusterConfigSource clusterConfigSource)
    {
        _provider = new VostokConfigurationProvider(clusterConfigSource);
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