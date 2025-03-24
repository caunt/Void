using Void.Proxy.Api.Events;

namespace Void.Proxy.Api.Plugins;

public interface IPlugin : IEventListener
{
    public string Name { get; }
}