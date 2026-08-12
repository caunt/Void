namespace Void.Proxy.Plugins.Common.Services.Lifecycle;

internal class KeepAliveTrackerRegistry<TKey> where TKey : class
{
    private readonly Lock _lock = new();
    private readonly Dictionary<TKey, KeepAliveTracker> _trackers = new(ReferenceEqualityComparer.Instance);

    public KeepAliveTracker GetOrAdd(TKey key, Func<KeepAliveTracker> createTracker)
    {
        using var _ = _lock.EnterScope();

        if (_trackers.TryGetValue(key, out var tracker))
            return tracker;

        tracker = createTracker();
        _trackers.Add(key, tracker);
        return tracker;
    }

    public KeepAliveTracker? Get(TKey key)
    {
        using var _ = _lock.EnterScope();
        return _trackers.GetValueOrDefault(key);
    }

    public bool IsCurrent(TKey key, KeepAliveTracker tracker)
    {
        using var _ = _lock.EnterScope();
        return _trackers.TryGetValue(key, out var currentTracker) && ReferenceEquals(currentTracker, tracker);
    }

    public KeepAliveTracker? Remove(TKey key)
    {
        using var _ = _lock.EnterScope();
        return _trackers.Remove(key, out var tracker) ? tracker : null;
    }
}
