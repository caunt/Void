namespace Void.Proxy.Plugins.Common.Network.Bundles;

public interface IBundleService
{
    public bool IsActivated { get; }

    public ValueTask WaitBundleCompletionAsync();
    public void ToggleBundle();
}
