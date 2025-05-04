namespace Void.Proxy.Plugins.ForwardingSupport.Velocity;

public class Settings
{
    public bool Enabled { get; set; } = true;
    public required string Secret { get; set; }
}
