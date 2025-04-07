namespace Void.Proxy.Plugins.Common.Network.Bundles;

public class BundleService : IBundleService
{
    private TaskCompletionSource? _taskCompletionSource;

    public bool IsActivated => _taskCompletionSource is { Task.IsCompleted: false };

    public async ValueTask WaitBundleCompletionAsync()
    {
        if (_taskCompletionSource is null)
            return;

        await _taskCompletionSource.Task;
    }

    public void ToggleBundle()
    {
        if (IsActivated)
        {
            _taskCompletionSource!.SetResult();
        }
        else
        {
            _taskCompletionSource = new TaskCompletionSource();
        }
    }
}
