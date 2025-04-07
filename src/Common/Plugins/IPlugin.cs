using Void.Common.Events;

namespace Void.Common.Plugins;

public interface IPlugin : IEventListener
{
    public string Name { get; }
}
