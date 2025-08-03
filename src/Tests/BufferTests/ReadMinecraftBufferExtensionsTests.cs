using System;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Components.Text.Properties;
using Void.Minecraft.Components.Text.Properties.Content;
using Void.Minecraft.Nbt.Tags;
using Void.Minecraft.Network;
using Void.Minecraft.Profiles;
using Xunit;

namespace Void.Tests.BufferTests;

public class ReadMinecraftBufferExtensionsTests
{
    [Fact]
    public void ReadUnsignedByte_ReturnsExpected()
    {
        Span<byte> data = stackalloc byte[1];
        var buffer = new BufferSpan(data);

        buffer.WriteUnsignedByte(0x42);
        buffer.Position = 0;

        var result = buffer.ReadUnsignedByte();

        Assert.Equal(0x42, result);
    }

    [Fact]
    public void ReadBoolean_ReadsTrueFalse()
    {
        Span<byte> data = stackalloc byte[2];
        var buffer = new BufferSpan(data);

        buffer.WriteBoolean(true);
        buffer.WriteBoolean(false);
        buffer.Position = 0;

        Assert.True(buffer.ReadBoolean());
        Assert.False(buffer.ReadBoolean());
    }

    [Fact]
    public void ReadVarInt_RoundTripsMultipleValues()
    {
        Span<byte> data = stackalloc byte[10];
        var buffer = new BufferSpan(data);

        buffer.WriteVarInt(300);
        buffer.WriteVarInt(-123);
        buffer.Position = 0;

        Assert.Equal(300, buffer.ReadVarInt());
        Assert.Equal(-123, buffer.ReadVarInt());
    }

    [Fact]
    public void ReadVarLong_RoundTrips()
    {
        Span<byte> data = stackalloc byte[16];
        var buffer = new BufferSpan(data);

        buffer.WriteVarLong(1234567890123L);
        buffer.Position = 0;

        Assert.Equal(1234567890123L, buffer.ReadVarLong());
    }

    [Fact]
    public void ReadShortVariants_RoundTrips()
    {
        Span<byte> data = stackalloc byte[8];
        var buffer = new BufferSpan(data);

        buffer.WriteUnsignedShort(65000);
        buffer.WriteShort(-12345);
        buffer.WriteVarShort(200000);
        buffer.Position = 0;

        Assert.Equal((ushort)65000, buffer.ReadUnsignedShort());
        Assert.Equal(-12345, buffer.ReadShort());
        Assert.Equal(200000, buffer.ReadVarShort());
    }

    [Fact]
    public void ReadIntAndLong_RoundTrips()
    {
        Span<byte> data = stackalloc byte[12];
        var buffer = new BufferSpan(data);

        buffer.WriteInt(int.MinValue);
        buffer.WriteLong(long.MaxValue);
        buffer.Position = 0;

        Assert.Equal(int.MinValue, buffer.ReadInt());
        Assert.Equal(long.MaxValue, buffer.ReadLong());
    }

    [Fact]
    public void ReadFloatAndDouble_RoundTrips()
    {
        Span<byte> data = stackalloc byte[12];
        var buffer = new BufferSpan(data);

        buffer.WriteFloat(123.5f);
        buffer.WriteDouble(456.25d);
        buffer.Position = 0;

        Assert.Equal(123.5f, buffer.ReadFloat());
        Assert.Equal(456.25d, buffer.ReadDouble());
    }

    [Fact]
    public void ReadString_RoundTrips()
    {
        Span<byte> data = stackalloc byte[32];
        var buffer = new BufferSpan(data);

        buffer.WriteString("minecraft");
        buffer.Position = 0;

        Assert.Equal("minecraft", buffer.ReadString());
    }

    [Fact]
    public void ReadUuid_RoundTrips()
    {
        Span<byte> data = stackalloc byte[32];
        var buffer = new BufferSpan(data);
        var uuid = Uuid.NewUuid();

        buffer.WriteUuid(uuid);
        buffer.Position = 0;

        Assert.Equal(uuid, buffer.ReadUuid());
    }

    [Fact]
    public void ReadUuidAsIntArray_RoundTrips()
    {
        Span<byte> data = stackalloc byte[32];
        var buffer = new BufferSpan(data);
        var uuid = Uuid.NewUuid();

        buffer.WriteUuidAsIntArray(uuid);
        buffer.Position = 0;

        Assert.Equal(uuid, buffer.ReadUuidAsIntArray());
    }

    [Fact]
    public void ReadProperty_RoundTrips()
    {
        Span<byte> data = stackalloc byte[64];
        var buffer = new BufferSpan(data);
        var property = new Property("name", "value", true, "sig");

        buffer.WriteProperty(property);
        buffer.Position = 0;

        var result = buffer.ReadProperty();
        Assert.Equal(property, result);
    }

    [Fact]
    public void ReadPropertyArray_RoundTrips()
    {
        Span<byte> data = stackalloc byte[128];
        var buffer = new BufferSpan(data);
        var properties = new[]
        {
            new Property("n1", "v1"),
            new Property("n2", "v2", true, "s2")
        };

        buffer.WritePropertyArray(properties);
        buffer.Position = 0;

        var result = buffer.ReadPropertyArray();
        Assert.Equal(properties, result);
    }

    [Fact]
    public void Dump_DoesNotAffectPosition()
    {
        Span<byte> data = stackalloc byte[8];
        var buffer = new BufferSpan(data);
        buffer.WriteInt(123);
        buffer.WriteInt(456);
        buffer.Position = 4;

        var dump = buffer.Dump();

        Assert.Equal(8, dump.Length);
        Assert.Equal(4, buffer.Position);
    }

    [Fact]
    public void ReadToEnd_ReadsRemainingBytes()
    {
        Span<byte> data = stackalloc byte[] { 1, 2, 3, 4, 5 };
        var buffer = new BufferSpan(data);
        buffer.Position = 1;

        var remaining = buffer.ReadToEnd();

        ReadOnlySpan<byte> expected = stackalloc byte[] { 2, 3, 4, 5 };
        Assert.True(expected.SequenceEqual(remaining));
        Assert.Equal(5, buffer.Position);
    }

    [Fact]
    public void ReadComponent_LegacyText_RoundTrips()
    {
        Span<byte> data = stackalloc byte[64];
        var buffer = new BufferSpan(data);
        var component = new Component(new TextContent("hello"), Children.Default, Formatting.Default, Interactivity.Default);

        buffer.WriteComponent(component, ProtocolVersion.MINECRAFT_1_20_2);
        buffer.Position = 0;

        var read = buffer.ReadComponent(ProtocolVersion.MINECRAFT_1_20_2);
        Assert.Equal("hello", read.AsText);
    }

    [Fact]
    public void ReadTag_RoundTripsCompound()
    {
        Span<byte> data = stackalloc byte[128];
        var buffer = new BufferSpan(data);
        var compound = new NbtCompound
        {
            ["name"] = new NbtString("value")
        };

        buffer.WriteTag(compound);
        buffer.Position = 0;

        var result = buffer.ReadTag();

        Assert.Equal(compound.ToString(), result.ToString());
    }

    [Fact]
    public void Read_ReadsExactBytesAndAdvances()
    {
        Span<byte> data = stackalloc byte[10];
        var buffer = new BufferSpan(data);

        buffer.Write([1, 2, 3, 4, 5]);
        buffer.Position = 0;

        var slice = buffer.Read(3);

        Assert.Equal(new byte[] { 1, 2, 3 }, slice.ToArray());
        Assert.Equal(3, buffer.Position);
    }
}
