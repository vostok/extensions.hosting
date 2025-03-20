using Vostok.ClusterConfig.Client.Abstractions;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Extensions.Hosting.Configuration;

public sealed class SubstitutableClusterConfigClient(IClusterConfigClient initialClient) : IClusterConfigClient
{
    private IClusterConfigClient _clusterConfigClient = initialClient;
    private readonly Dictionary<ClusterConfigPath, SubstitutableObservable<ISettingsNode>> _observables = [];

    public static SubstitutableClusterConfigClient Instance { get; set; }

    public IObservable<ISettingsNode> Observe(ClusterConfigPath prefix)
    {
        lock (_observables)
        {
            if (_observables.TryGetValue(prefix, out var substitutableObservable))
                return substitutableObservable;

            var baseObservable = _clusterConfigClient.Observe(prefix);
            substitutableObservable = new SubstitutableObservable<ISettingsNode>(baseObservable);
            _observables.Add(prefix, substitutableObservable);

            return substitutableObservable;
        }
    }

    public void SubstituteWith(IClusterConfigClient newClient)
    {
        lock (_observables)
        {
            var oldClient = _clusterConfigClient;

            _clusterConfigClient = newClient;
            foreach (var (path, substitutableObservable) in _observables)
            {
                var observable = _clusterConfigClient.Observe(path);
                substitutableObservable.SubstituteWith(observable);
            }

            (oldClient as IDisposable)?.Dispose();
        }
    }

    private sealed class SubstitutableObservable<T>(IObservable<T> observable) : IObservable<T>
    {
        private IObservable<T> _observable = observable;
        private readonly List<(IObserver<T>, IDisposable)> _observers = [];

        public IDisposable Subscribe(IObserver<T> observer)
        {
            lock (_observers)
            {
                var subscription = _observable.Subscribe(observer);
                _observers.Add((observer, subscription));

                return subscription;
            }
        }

        public void SubstituteWith(IObservable<T> newObservable)
        {
            lock (_observers)
            {
                _observable = newObservable;
                foreach (var (observer, subscription) in _observers)
                {
                    subscription.Dispose();
                    _observable.Subscribe(observer);
                }
            }
        }
    }

    public ISettingsNode Get(ClusterConfigPath prefix) => throw new NotSupportedException();
    public (ISettingsNode settings, long version) GetWithVersion(ClusterConfigPath prefix) => throw new NotSupportedException();
    public Task<ISettingsNode> GetAsync(ClusterConfigPath prefix) => throw new NotSupportedException();
    public Task<(ISettingsNode settings, long version)> GetWithVersionAsync(ClusterConfigPath prefix) => throw new NotSupportedException();
    public IObservable<(ISettingsNode settings, long version)> ObserveWithVersions(ClusterConfigPath prefix) => throw new NotSupportedException();
}