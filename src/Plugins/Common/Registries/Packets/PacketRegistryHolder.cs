using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.API.Network;
using Void.Proxy.API.Plugins;
using Void.Proxy.Plugins.Common.Network.IO.Messages;
using Void.Proxy.Plugins.Common.Network.Protocol.Packets;

namespace Void.Proxy.Plugins.Common.Registries.Packets;

public class PacketRegistryHolder : IPacketRegistryHolder
{
    public bool IsEmpty => this is { Read.IsEmpty: true, Write.IsEmpty: true, ManagedBy: null };
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IPacketRegistry Read { get; set; } = new PacketRegistry();
    public IPacketRegistry Write { get; set; } = new PacketRegistry();

    public bool Contains<T>() where T : IMinecraftPacket
    {
        return Read.Contains<T>() || Write.Contains<T>();
    }

    public void ReplacePackets(Operation operation, IReadOnlyDictionary<PacketMapping[], Type> mappings)
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

    public void AddPackets(Operation operation, IReadOnlyDictionary<PacketMapping[], Type> mappings)
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