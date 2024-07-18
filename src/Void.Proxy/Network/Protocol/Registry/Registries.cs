namespace Void.Proxy.Network.Protocol.Registry;

public static class Registries
{
    public static readonly StateRegistry HandshakeStateRegistry = new();
    public static readonly StateRegistry LoginStateRegistry = new();
    public static readonly StateRegistry ConfigurationStateRegistry = new();
    public static readonly StateRegistry PlayStateRegistry = new();

    public static void Fill()
    {
        // HandshakeStateRegistry.Serverbound.Register<HandshakePacket>(() => new(), new PacketMapping(0x00, false, ProtocolVersion.MINECRAFT_1_7_2));
    }
}