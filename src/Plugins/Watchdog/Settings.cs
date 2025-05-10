namespace Void.Proxy.Plugins.Watchdog;

public record Settings(bool Enabled = false, string Address = "*", int Port = 80);
