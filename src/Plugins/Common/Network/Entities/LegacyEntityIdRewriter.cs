using System.Buffers.Binary;
using System.Runtime.InteropServices;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Binary;
using Void.Proxy.Api.Network;

namespace Void.Proxy.Plugins.Common.Network.Entities;

// Packet layouts follow the Minecraft protocol history and BungeeCord's EntityMap implementation.
public static class LegacyEntityIdRewriter
{
    public static void Rewrite(IMinecraftBinaryMessage message, ProtocolVersion protocolVersion, Direction direction, int serverEntityId, int clientEntityId)
    {
        if (serverEntityId == clientEntityId)
            return;

        var profile = GetProfile(protocolVersion);

        if (profile is null)
            return;

        if (direction is Direction.Serverbound && !profile.ServerboundVarIntPackets.Contains(message.Id))
            return;

        if (direction is Direction.Clientbound && !IsClientboundPacket(message.Id, profile))
            return;

        var stream = message.Stream;
        var originalPosition = stream.Position;
        var bytes = stream.ToArray().ToList();
        var offset = checked((int)originalPosition);

        bool changed;
        if (direction is Direction.Serverbound)
        {
            changed = RewriteVarInt(bytes, offset, serverEntityId, clientEntityId);
        }
        else if (direction is Direction.Clientbound)
        {
            changed = RewriteClientbound(bytes, offset, message.Id, profile, serverEntityId, clientEntityId);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }

        if (!changed)
            return;

        stream.SetLength(0);
        stream.Position = 0;
        stream.Write([.. bytes]);
        stream.Position = originalPosition;
    }

    private static bool RewriteClientbound(List<byte> bytes, int offset, int packetId, Profile profile, int serverEntityId, int clientEntityId)
    {
        var changed = false;

        if (profile.ClientboundVarIntPackets.Contains(packetId))
            changed = RewriteVarInt(bytes, offset, serverEntityId, clientEntityId);
        else if (profile.ClientboundIntPackets.Contains(packetId))
            changed = RewriteInt(bytes, offset, serverEntityId, clientEntityId);

        if (packetId == profile.AttachEntityPacket)
        {
            changed |= RewriteInt(bytes, offset + sizeof(int), serverEntityId, clientEntityId);
        }
        else if (packetId == profile.CollectItemPacket)
        {
            var secondEntityOffset = SkipVarInt(bytes, offset);
            changed |= RewriteVarInt(bytes, secondEntityOffset, serverEntityId, clientEntityId);
        }
        else if (packetId == profile.SetPassengersPacket)
        {
            var countOffset = SkipVarInt(bytes, offset);
            changed |= RewriteVarIntArray(bytes, countOffset, serverEntityId, clientEntityId);
        }
        else if (packetId == profile.DestroyEntitiesPacket)
        {
            changed |= RewriteVarIntArray(bytes, offset, serverEntityId, clientEntityId);
        }
        else if (packetId == profile.EntitySoundPacket)
        {
            var soundCategoryOffset = SkipVarInt(bytes, offset);
            var entityOffset = SkipVarInt(bytes, soundCategoryOffset);
            changed |= RewriteVarInt(bytes, entityOffset, serverEntityId, clientEntityId);
        }

        return changed;
    }

    private static bool RewriteVarIntArray(List<byte> bytes, int countOffset, int serverEntityId, int clientEntityId)
    {
        var count = ReadVarInt(bytes, countOffset, out var countLength);
        var offset = countOffset + countLength;
        var changed = false;

        for (var index = 0; index < count; index++)
        {
            changed |= RewriteVarInt(bytes, offset, serverEntityId, clientEntityId);
            offset = SkipVarInt(bytes, offset);
        }

        return changed;
    }

    private static bool RewriteInt(List<byte> bytes, int offset, int serverEntityId, int clientEntityId)
    {
        if (offset < 0 || offset + sizeof(int) > bytes.Count)
            return false;

        var value = BinaryPrimitives.ReadInt32BigEndian(CollectionsMarshal.AsSpan(bytes)[offset..]);
        var replacement = Swap(value, serverEntityId, clientEntityId);

        if (replacement == value)
            return false;

        BinaryPrimitives.WriteInt32BigEndian(CollectionsMarshal.AsSpan(bytes)[offset..], replacement);
        return true;
    }

    private static bool RewriteVarInt(List<byte> bytes, int offset, int serverEntityId, int clientEntityId)
    {
        if (offset < 0 || offset >= bytes.Count)
            return false;

        var value = ReadVarInt(bytes, offset, out var length);
        var replacement = Swap(value, serverEntityId, clientEntityId);

        if (replacement == value)
            return false;

        var replacementBytes = replacement.AsVarInt();
        bytes.RemoveRange(offset, length);
        bytes.InsertRange(offset, replacementBytes);
        return true;
    }

    private static int SkipVarInt(List<byte> bytes, int offset)
    {
        _ = ReadVarInt(bytes, offset, out var length);
        return offset + length;
    }

    private static int ReadVarInt(List<byte> bytes, int offset, out int length)
    {
        if (offset < 0 || offset >= bytes.Count)
            throw new InvalidDataException("Cannot read an entity ID outside the packet payload.");

        var value = 0;
        length = 0;

        while (true)
        {
            if (offset + length >= bytes.Count || length == 5)
                throw new InvalidDataException("Entity ID contains an invalid VarInt.");

            var current = bytes[offset + length];
            value |= (current & 0x7F) << (7 * length++);

            if ((current & 0x80) == 0)
                return value;
        }
    }

    private static int Swap(int value, int serverEntityId, int clientEntityId)
    {
        if (value == serverEntityId)
            return clientEntityId;

        if (value == clientEntityId)
            return serverEntityId;

        return value;
    }

    private static bool IsClientboundPacket(int packetId, Profile profile)
    {
        return profile.ClientboundVarIntPackets.Contains(packetId) ||
               profile.ClientboundIntPackets.Contains(packetId) ||
               packetId == profile.DestroyEntitiesPacket ||
               packetId == profile.EntitySoundPacket;
    }

    private static Profile? GetProfile(ProtocolVersion protocolVersion)
    {
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_15 && protocolVersion <= ProtocolVersion.MINECRAFT_1_15_2)
            return Profiles.V1_15;

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_14 && protocolVersion <= ProtocolVersion.MINECRAFT_1_14_4)
            return Profiles.V1_14;

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_13 && protocolVersion <= ProtocolVersion.MINECRAFT_1_13_2)
            return Profiles.V1_13;

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_12_1 && protocolVersion <= ProtocolVersion.MINECRAFT_1_12_2)
            return Profiles.V1_12_1;

        if (protocolVersion == ProtocolVersion.MINECRAFT_1_12)
            return Profiles.V1_12;

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_9_3 && protocolVersion <= ProtocolVersion.MINECRAFT_1_11_1)
            return Profiles.V1_9_4;

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_9 && protocolVersion <= ProtocolVersion.MINECRAFT_1_9_2)
            return Profiles.V1_9;

        if (protocolVersion == ProtocolVersion.MINECRAFT_1_8)
            return Profiles.V1_8;

        return null;
    }

    private sealed record Profile(HashSet<int> ClientboundVarIntPackets, HashSet<int> ClientboundIntPackets, HashSet<int> ServerboundVarIntPackets, int AttachEntityPacket, int CollectItemPacket, int SetPassengersPacket, int DestroyEntitiesPacket, int EntitySoundPacket = -1);

    private static class Profiles
    {
        public static readonly Profile V1_8 = Create(
            [0x04, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x12, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1C, 0x1D, 0x1E, 0x20, 0x25, 0x2C, 0x43, 0x49],
            [0x1A, 0x1B], [0x02, 0x0B], 0x1B, 0x0D, -1, 0x13);

        public static readonly Profile V1_9 = Create(
            [0x00, 0x01, 0x03, 0x04, 0x05, 0x06, 0x08, 0x25, 0x26, 0x27, 0x28, 0x2F, 0x31, 0x34, 0x36, 0x39, 0x3B, 0x3C, 0x40, 0x49, 0x4A, 0x4B, 0x4C],
            [0x1B, 0x3A], [0x0A, 0x14], 0x3A, 0x49, 0x40, 0x30);

        public static readonly Profile V1_9_4 = Create(
            [0x00, 0x01, 0x03, 0x04, 0x05, 0x06, 0x08, 0x25, 0x26, 0x27, 0x28, 0x2F, 0x31, 0x34, 0x36, 0x39, 0x3B, 0x3C, 0x40, 0x48, 0x49, 0x4A, 0x4B],
            [0x1B, 0x3A], [0x0A, 0x14], 0x3A, 0x48, 0x40, 0x30);

        public static readonly Profile V1_12 = Create(
            [0x00, 0x01, 0x03, 0x04, 0x05, 0x06, 0x08, 0x25, 0x26, 0x27, 0x28, 0x2F, 0x32, 0x35, 0x38, 0x3B, 0x3D, 0x3E, 0x42, 0x4A, 0x4B, 0x4D, 0x4E],
            [0x1B, 0x3C], [0x0B, 0x15], 0x3C, 0x4A, 0x42, 0x31);

        public static readonly Profile V1_12_1 = Create(
            [0x00, 0x01, 0x03, 0x04, 0x05, 0x06, 0x08, 0x25, 0x26, 0x27, 0x28, 0x30, 0x33, 0x36, 0x39, 0x3C, 0x3E, 0x3F, 0x43, 0x4B, 0x4C, 0x4E, 0x4F],
            [0x1B, 0x3D], [0x0A, 0x15], 0x3D, 0x4B, 0x43, 0x32);

        public static readonly Profile V1_13 = Create(
            [0x00, 0x01, 0x03, 0x04, 0x05, 0x06, 0x08, 0x27, 0x28, 0x29, 0x2A, 0x33, 0x36, 0x39, 0x3C, 0x3F, 0x41, 0x42, 0x46, 0x4F, 0x50, 0x52, 0x53],
            [0x1C, 0x40], [0x0D, 0x19], 0x40, 0x4F, 0x46, 0x35);

        public static readonly Profile V1_14 = Create(
            [0x00, 0x01, 0x03, 0x04, 0x05, 0x06, 0x08, 0x28, 0x29, 0x2A, 0x2B, 0x38, 0x3B, 0x3E, 0x43, 0x45, 0x46, 0x4A, 0x55, 0x56, 0x58, 0x59],
            [0x1B, 0x44], [0x0E, 0x1B], 0x44, 0x55, 0x4A, 0x37, 0x50);

        public static readonly Profile V1_15 = Create(
            [0x00, 0x01, 0x03, 0x04, 0x05, 0x06, 0x09, 0x29, 0x2A, 0x2B, 0x2C, 0x39, 0x3C, 0x3F, 0x44, 0x46, 0x47, 0x4B, 0x56, 0x57, 0x59, 0x5A],
            [0x1C, 0x45], [0x0E, 0x1B], 0x45, 0x56, 0x4B, 0x38, 0x51);

        private static Profile Create(int[] clientboundVarIntPackets, int[] clientboundIntPackets, int[] serverboundVarIntPackets, int attachEntityPacket, int collectItemPacket, int setPassengersPacket, int destroyEntitiesPacket, int entitySoundPacket = -1)
        {
            return new Profile([.. clientboundVarIntPackets], [.. clientboundIntPackets], [.. serverboundVarIntPackets], attachEntityPacket, collectItemPacket, setPassengersPacket, destroyEntitiesPacket, entitySoundPacket);
        }
    }
}
