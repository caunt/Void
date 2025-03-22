namespace Void.Proxy.Plugins.Common.Network.IO.Bundles;

public interface IBundleService
{
    public bool IsActivated { get; }

    public ValueTask WaitBundleCompletionAsync();
    public void ToggleBundle();
}
