using Void.Minecraft.Network;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.IO.Messages.Packets;
using Void.Proxy.Api.Network.IO.Streams.Packet;
using Void.Proxy.Api.Network.IO.Streams.Packet.Registries;
using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Plugins.Common.Network.IO.Streams.Packet.Registries;

public class MinecraftPacketRegistrySystem : IMinecraftPacketRegistrySystem
{
    public bool IsEmpty => this is { Read.IsEmpty: true, Write.IsEmpty: true, ManagedBy: null };
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IMinecraftPacketRegistry Read { get; set; } = new MinecraftPacketRegistry();
    public IMinecraftPacketRegistry Write { get; set; } = new MinecraftPacketRegistry();

    public bool Contains<T>() where T : IMinecraftPacket
    {
        return Read.Contains<T>() || Write.Contains<T>();
    }

    public void ReplacePackets(Operation operation, IReadOnlyDictionary<MinecraftPacketMapping[], Type> mappings)
    {
        if (ProtocolVersion is null)
            throw new InvalidOperationException("Protocol version is not set yet");

        switch (operation)
        {
            case Operation.Read:
                Read.ReplacePackets(mappings, ProtocolVersion);
                break;
            case Operation.Write:
                Write.ReplacePackets(mappings, ProtocolVersion);
                break;
            case Operation.Any:
                Read.ReplacePackets(mappings, ProtocolVersion);
                Write.ReplacePackets(mappings, ProtocolVersion);
                break;
            default:
                throw new ArgumentException("Invalid operation", nameof(operation));
        }
    }

    public void AddPackets(Operation operation, IReadOnlyDictionary<MinecraftPacketMapping[], Type> mappings)
    {
        if (ProtocolVersion is null)
            throw new InvalidOperationException("Protocol version is not set yet");

        switch (operation)
        {
            case Operation.Read:
                Read.AddPackets(mappings, ProtocolVersion);
                break;
            case Operation.Write:
                Write.AddPackets(mappings, ProtocolVersion);
                break;
            case Operation.Any:
                Read.AddPackets(mappings, ProtocolVersion);
                Write.AddPackets(mappings, ProtocolVersion);
                break;
            default:
                throw new ArgumentException("Invalid operation", nameof(operation));
        }
    }

    public void Reset()
    {
        ProtocolVersion = null;
        ManagedBy = null;

        Read.Clear();
        Write.Clear();
    }
}