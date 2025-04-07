namespace Void.Common;

public interface IPlugin : IEventListener
{
    public string Name { get; }
}
