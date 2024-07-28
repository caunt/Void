using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Network.Protocol.States;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.States;

public abstract class ProtocolState : IProtocolState
{
    public ProtocolVersion ProtocolVersion { get; }

    // public IMinecraftPacket<T>? Decode<T>(int packetId, Direction direction, ref MinecraftBuffer buffer, ProtocolVersion protocolVersion) where T : ProtocolState
    // {
    //     var protocolRegistry = Registries.GetProtocolRegistry(direction, protocolVersion);
    //     var packet = protocolRegistry.CreatePacket(packetId);
    // 
    //     if (packet is null)
    //         return null; // packet not registered, proceed
    // 
    //     packet.Decode(ref buffer, protocolVersion);
    // 
    //     if (buffer.HasData)
    //         throw new IOException($"{direction} packet {packet} has extra data ({buffer.Position} < {buffer.Length})");
    // 
    //     return packet as IMinecraftPacket<T> ?? throw new Exception($"Cannot cast instance of {packetId} packet to {typeof(IMinecraftPacket<T>)}");
    // }
    // 
    // public int? FindPacketId(Direction direction, IMinecraftPacket packet, ProtocolVersion protocolVersion)
    // {
    //     return Registries.GetProtocolRegistry(direction, protocolVersion)
    //         .GetPacketId(packet);
    // }
}