namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Commands;

public interface IChatCommand
{
    public bool IsSigned { get; }
    public string Command { get; set; }
}
