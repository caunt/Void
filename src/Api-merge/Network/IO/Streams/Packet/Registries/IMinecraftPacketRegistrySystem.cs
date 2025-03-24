using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Messages.Packets;
using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Registries;

public interface IMinecraftPacketRegistrySystem
{
    public bool IsEmpty { get; }
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IMinecraftPacketRegistry Read { get; set; }
    public IMinecraftPacketRegistry Write { get; set; }

    public bool Contains<T>() where T : IMinecraftPacket;
    public void ReplacePackets(Operation operation, IReadOnlyDictionary<MinecraftPacketMapping[], Type> mappings);
    public void AddPackets(Operation operation, IReadOnlyDictionary<MinecraftPacketMapping[], Type> mappings);
    public void Reset();
}